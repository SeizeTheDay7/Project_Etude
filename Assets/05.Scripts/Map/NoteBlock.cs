using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBlock : MonoBehaviour
{
    public KeyType requiredKeys;
    public int noteBlockIndex;
    public GameObject prevNoteBlock;
    public GameObject nextNoteBlock;

    public void RemoveBlock()
    {
        // Debug.Log($"Removing: {gameObject.name}");
        Destroy(this.gameObject);
    }
}
