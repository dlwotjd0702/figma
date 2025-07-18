using UnityEngine;
using UnityEditor;
using TMPro;

public class FontBatchReplacer : EditorWindow
{
    private static readonly string TMP_FONT_PATH = "Assets/Resources/Font/Noto_Sans_KR/static/NotoSansKR-Regular SDF.asset";
    private static TMP_FontAsset cachedTMPFont;

    [MenuItem("Tools/전체 TMP 텍스트 일괄설정(NotoSansKR, 중앙정렬, 랩핑끄기)")]
    public static void ReplaceAllTMPFontsAndAlignment()
    {
        // TMP FontAsset 로드
        if (cachedTMPFont == null)
            cachedTMPFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(TMP_FONT_PATH);

        if (cachedTMPFont == null)
        {
            EditorUtility.DisplayDialog("경고", $"TMP FontAsset을 찾을 수 없음!\n경로: {TMP_FONT_PATH}", "확인");
            return;
        }

        int changed = 0;

        foreach (var tmp in GameObject.FindObjectsOfType<TextMeshProUGUI>(true))
        {
            Undo.RecordObject(tmp, "TMP 폰트/정렬/랩핑변경");

            // 1. 폰트 및 머티리얼
            tmp.font = cachedTMPFont;
            tmp.fontMaterial = cachedTMPFont.material;

            // 2. 정렬 (가운데/가운데)
            tmp.alignment = TextAlignmentOptions.Center | TextAlignmentOptions.Midline;

            // 3. 워드랩/오버플로우 설정 (한줄 강제, 글씨 짤리지 않게)
            tmp.enableWordWrapping = false;
            tmp.overflowMode = TextOverflowModes.Overflow;

            // 4. 기타 권장 설정
            tmp.fontStyle = FontStyles.Normal;
            tmp.ForceMeshUpdate();

            changed++;
        }

        Debug.Log($"NotoSansKR-Regular SDF + 중앙정렬 + 랩핑 OFF 일괄적용: {changed}개");
        EditorUtility.DisplayDialog("완료", $"NotoSansKR-Regular SDF, 중앙정렬, 랩핑OFF 일괄적용!\nTMP: {changed}", "OK");
    }
}