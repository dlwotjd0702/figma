using UnityEngine;
using UnityEditor;
using TMPro;
using UnityFigmaBridge.Runtime.UI;

public class TMP_FontSetter : EditorWindow
{
    private const string TMP_FONT_PATH = "Assets/Fonts/army Bold SDF.asset";

    [MenuItem("Tools/TMP 폰트 일괄 적용툴")]
    public static void ShowWindow()
    {
        GetWindow<TMP_FontSetter>(false, "TMP 폰트 일괄 적용");
    }

    private void OnGUI()
    {
        GUILayout.Label("씬 내 모든 TextMeshProUGUI 폰트 일괄 적용", EditorStyles.boldLabel);
        if (GUILayout.Button("폰트 적용 실행", GUILayout.Height(35)))
        {
            ApplyFont();
        }
    }

    private static void ApplyFont()
    {
        TMP_FontAsset targetFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(TMP_FONT_PATH);
        if (targetFont == null)
        {
            EditorUtility.DisplayDialog("에러", $"TMP 폰트 에셋을 찾을 수 없습니다.\n경로: {TMP_FONT_PATH}", "확인");
            return;
        }

        var tmps = GameObject.FindObjectsOfType<TextMeshProUGUI>(true);
        int count = 0;

        foreach (var tmp in tmps)
        {
            if (tmp != null && tmp.font != targetFont)
            {
                Undo.RecordObject(tmp, "TMP 폰트 일괄 적용");
                tmp.font = targetFont;
                count++;
            }
        }

        EditorUtility.DisplayDialog("폰트 적용 완료", $"변경된 TMP 오브젝트 개수: {count}", "확인");
    }
}



public class TMP_SubMeshUI_AndFigmaFlowRemover : EditorWindow
{
    [MenuItem("Tools/TMP SubMeshUI + FigmaFlowButton 일괄삭제툴")]
    public static void ShowWindow()
    {
        GetWindow<TMP_SubMeshUI_AndFigmaFlowRemover>(false, "TMP+Figma 삭제");
    }

    private void OnGUI()
    {
        GUILayout.Label("씬 내 TMP_SubMeshUI 오브젝트와 FigmaPrototypeFlowButton 컴포넌트 일괄 삭제", EditorStyles.boldLabel);
        if (GUILayout.Button("일괄 삭제 실행", GUILayout.Height(40)))
        {
            RemoveAll();
        }
    }

    private static void RemoveAll()
    {
        // 1. TMP_SubMeshUI 컴포넌트가 붙은 오브젝트 전체 삭제
        var subs = Resources.FindObjectsOfTypeAll<TMP_SubMeshUI>();
        int tmpCount = 0;
        foreach (var sub in subs)
        {
            if (sub != null && sub.gameObject != null &&
                !EditorUtility.IsPersistent(sub.gameObject) && // 씬 오브젝트만
                !(sub.gameObject.hideFlags.HasFlag(HideFlags.NotEditable) || sub.gameObject.hideFlags.HasFlag(HideFlags.HideAndDontSave)))
            {
                Undo.DestroyObjectImmediate(sub.gameObject);
                tmpCount++;
            }
        }

        // 2. FigmaPrototypeFlowButton 컴포넌트만 제거
#if UNITY_EDITOR
        int figmaCount = 0;
        var figmaBtns = Resources.FindObjectsOfTypeAll<FigmaPrototypeFlowButton>();
        foreach (var btn in figmaBtns)
        {
            if (btn != null && btn.gameObject != null &&
                !EditorUtility.IsPersistent(btn.gameObject) &&
                !(btn.gameObject.hideFlags.HasFlag(HideFlags.NotEditable) || btn.gameObject.hideFlags.HasFlag(HideFlags.HideAndDontSave)))
            {
                Undo.DestroyObjectImmediate(btn);
                figmaCount++;
            }
        }
#else
        int figmaCount = 0;
#endif

        EditorUtility.DisplayDialog(
            "일괄 삭제 완료",
            $"TMP_SubMeshUI 오브젝트 삭제: {tmpCount}개\n" +
            $"FigmaPrototypeFlowButton 컴포넌트 삭제: {figmaCount}개",
            "확인"
        );
    }
}



