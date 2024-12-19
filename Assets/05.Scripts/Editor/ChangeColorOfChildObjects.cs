using UnityEngine;
using UnityEditor;

public class SpriteColorChanger : EditorWindow
{
    private Color targetColor = Color.white; // 변경할 색상
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

        // 색상 선택
        targetColor = EditorGUILayout.ColorField("Target Color", targetColor);

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
            // 색상이 검정색인지 확인
            if (sr.color == Color.black)
            {
                Undo.RecordObject(sr, "Change Sprite Color"); // Undo 기록
                sr.color = targetColor; // 색상 변경
                EditorUtility.SetDirty(sr); // 변경 사항 저장
                changedCount++;
            }
        }

        Debug.Log($"Changed color of {changedCount} SpriteRenderer(s) from black to {targetColor}.");
    }
}
