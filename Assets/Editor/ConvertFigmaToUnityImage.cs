// Assets/Editor/ConvertFigmaToUnityUGUI.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class ConvertFigmaToUnityUGUI : EditorWindow
{
    [MenuItem("Tools/Figma → UGUI 변환기 (FillColor Fix)")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ConvertFigmaToUnityUGUI), false, "Figma → UGUI 변환기");
    }

    private void OnGUI()
    {
        GUILayout.Label("Figma Bridge UI → Unity UGUI 변환기", EditorStyles.boldLabel);
        if (GUILayout.Button("씬/프리팹 전체 변환 실행"))
        {
            ConvertAll();
        }
    }

    public static void ConvertAll()
    {
        var allGos = GameObject.FindObjectsOfType<GameObject>(true);

        int imgSprite = 0, imgDefault = 0, imgOnly = 0;
        int btnConverted = 0, compRemoved = 0, missingRemoved = 0, failed = 0;

        foreach (var go in allGos)
        {
            var comps = go.GetComponents<Component>();
            foreach (var c in comps)
            {
                if (c == null) continue;
                var t = c.GetType();
                string tname = t.FullName;

                if (tname == "UnityFigmaBridge.Runtime.UI.FigmaImage" ||
                    tname == "UnityFigmaBridge.Runtime.UI.FigmaRectangle" ||
                    tname == "UnityFigmaBridge.Runtime.UI.FigmaVector")
                {
                    try
                    {
                        // 1. 스프라이트, FillColor 백업
                        Sprite sprite = null;
                        Color fillColor = Color.white;
                        var spriteProp = t.GetProperty("sprite");
                        var fillColorField = t.GetField("m_FillColor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                        if (spriteProp != null) sprite = spriteProp.GetValue(c) as Sprite;
                        if (fillColorField != null) fillColor = (Color)fillColorField.GetValue(c);

                        // 2. FigmaImage/Rectangle/Vector 컴포넌트 삭제
                        Undo.DestroyObjectImmediate(c);

                        // 3. Image 컴포넌트 추가 (없으면)
                        var img = go.GetComponent<Image>();
                        if (img == null) img = Undo.AddComponent<Image>(go);

                        // 4. fillColor/sprite 할당
                        img.color = fillColor;

                        if (sprite != null)
                        {
                            img.sprite = sprite;
                            img.type = Image.Type.Simple;
                            imgSprite++;
                        }
                        else
                        {
                            Sprite def = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
                            if (def != null)
                            {
                                img.sprite = def;
                                img.type = Image.Type.Sliced;
                                imgDefault++;
                            }
                            else
                            {
                                img.sprite = null;
                                imgOnly++;
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning($"[Figma→UGUI] Image류 변환 실패: {go.name} ({e.Message})");
                        failed++;
                    }
                }
                // 이하 생략(동일)
                // 버튼/불필요컴포넌트/미싱스 제거 등 이전코드와 동일하게 사용하면 됨
            }
        }
        // 이하 결과 출력 등 동일
        AssetDatabase.Refresh();
    }
}




public class RemoveContentSizeFitterAndSetWidth : EditorWindow
{
    [MenuItem("Tools/ContentSizeFitter 제거+Rect Width 300")]
    public static void ShowWindow()
    {
        GetWindow<RemoveContentSizeFitterAndSetWidth>(false, "CSF 제거+Rect 300");
    }

    private void OnGUI()
    {
        GUILayout.Label("씬 내 ContentSizeFitter 제거 및 RectTransform width=300 적용", EditorStyles.boldLabel);
        if (GUILayout.Button("실행", GUILayout.Height(35)))
        {
            RemoveAndResize();
        }
    }

    private static void RemoveAndResize()
    {
        var fitters = GameObject.FindObjectsOfType<ContentSizeFitter>(true);
        int count = 0;

        foreach (var fitter in fitters)
        {
            if (fitter == null) continue;

            var go = fitter.gameObject;
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) continue;

            Undo.RegisterCompleteObjectUndo(go, "ContentSizeFitter 제거+Rect 300");

            // ContentSizeFitter 제거
            Undo.DestroyObjectImmediate(fitter);

            // width(넓이)만 300으로 고정
            var size = rect.sizeDelta;
            size.x = 300;
            rect.sizeDelta = size;

            count++;
        }

        EditorUtility.DisplayDialog("완료", $"변경된 개수: {count}\n(ContentSizeFitter 제거 및 width=300 적용)", "확인");
    }
}
