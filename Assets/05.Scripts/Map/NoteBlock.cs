using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBlock : MonoBehaviour
{
    public KeyType requiredKeys;
    public int noteBlockIndex;
    public GameObject prevNoteBlock;
    public GameObject nextNoteBlock;

    public void DisableCollider()
    {
        GetComponentInChildren<BoxCollider>().enabled = false;

        // 자식에 있는 모든 스프라이트 렌더러를 찾아서 만약 검은색이라면 alpha 값 180으로 설정
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            Color color = spriteRenderer.color;
            if (color == Color.black)
            {
                color.a = 0.7f;
                spriteRenderer.color = color;
            }
        }
    }

    public void EnableCollider()
    {
        GetComponentInChildren<BoxCollider>().enabled = true;

        // 자식에 있는 모든 스프라이트 렌더러를 찾아서 만약 검은색이라면 alpha 값 255로 설정
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }
    }
}
