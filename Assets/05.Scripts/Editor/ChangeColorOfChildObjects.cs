using UnityEngine;
using UnityEditor;

public class SpriteColorChanger : EditorWindow
{

    private Color targetColor1; // 변경될 색상
    private Color targetColor2;
    private Color goalColor1; // 변경할 색상
    private Color goalColor2;
    private GameObject parentObject; // 부모 오브젝트

    [MenuItem("Tools/Sprite Color Changer")]
    public static void ShowWindow()
    {
        GetWindow<SpriteColorChanger>("Sprite Color Changer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Sprite Color Changer", EditorStyles.boldLabel);

        // 부모 오브젝트 선택
        parentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(GameObject), true);


        // sprite renderer 다 뒤져서 color들 찾아낸 후에 2개를 각각 할당
        foreach (SpriteRenderer sr in parentObject.GetComponentsInChildren<SpriteRenderer>())
        {
            if (targetColor1 == Color.clear || targetColor1 == sr.color)
            {
                targetColor1 = sr.color;
            }
            else if (targetColor2 == Color.clear || targetColor2 == sr.color)
            {
                targetColor2 = sr.color;
            }
        }
        Debug.Log("targetColor1: " + targetColor1);
        Debug.Log("targetColor2: " + targetColor2);

        // 색상 선택
        goalColor1 = EditorGUILayout.ColorField("Goal Color1", goalColor1);
        goalColor2 = EditorGUILayout.ColorField("Goal Color2", goalColor2);

        if (GUILayout.Button("Change Colors"))
        {
            ChangeSpriteColors();
        }
    }

    private void ChangeSpriteColors()
    {
        if (parentObject == null)
        {
            Debug.LogError("Parent Object is not set!");
            return;
        }

        // 모든 하위 SpriteRenderer 찾기
        SpriteRenderer[] spriteRenderers = parentObject.GetComponentsInChildren<SpriteRenderer>();

        if (spriteRenderers.Length == 0)
        {
            Debug.LogWarning("No SpriteRenderers found under the selected object.");
            return;
        }

        int changedCount = 0; // 변경된 SpriteRenderer 개수

        foreach (SpriteRenderer sr in spriteRenderers)
        {
            if (sr.color == targetColor1)
            {
                Undo.RecordObject(sr, "Change Sprite Color"); // Undo 기록
                sr.color = goalColor1; // 색상 변경
                EditorUtility.SetDirty(sr); // 변경 사항 저장
                changedCount++;
            }
            if (sr.color == targetColor2)
            {
                Undo.RecordObject(sr, "Change Sprite Color"); // Undo 기록
                sr.color = goalColor2; // 색상 변경
                EditorUtility.SetDirty(sr); // 변경 사항 저장
                changedCount++;
            }
        }
        Debug.Log(changedCount + " SpriteRenderers changed color.");
    }
}
