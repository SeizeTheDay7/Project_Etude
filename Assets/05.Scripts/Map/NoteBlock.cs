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
    }

    public void EnableCollider()
    {
        GetComponentInChildren<BoxCollider>().enabled = true;
    }
}
