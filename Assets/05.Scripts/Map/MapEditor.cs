using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MapEditor : MonoBehaviour
{
    GameObject NewBlock;
    [SerializeField] GameObject WholeNote;
    [SerializeField] GameObject HalfNote;
    [SerializeField] GameObject QuarterNote;
    [SerializeField] GameObject EighthNote;
    [SerializeField] GameObject SixteenthNote;

    [SerializeField] private Button createButton;
    [SerializeField] private Button changeButton;
    [SerializeField] private Button deleteButton;
    private Button saveButton;

    [SerializeField] TMP_Dropdown DurationDropdown;
    [SerializeField] TMP_Dropdown DirectionDropdown;

    [SerializeField] GameObject ObjectSelector;

    void Start()
    {
        InitButtonEvent();
    }

    void InitButtonEvent()
    {
        createButton.onClick.AddListener(CreateNewBlock);
        changeButton.onClick.AddListener(ChangeBlock);
        deleteButton.onClick.AddListener(DeleteBlock);
    }

    void CreateNewBlock()
    {
        SetBlockTypeAndInstantiate();
        SetBlockDirection();
    }

    void ChangeBlock()
    {
        GameObject SelectedBlock = ObjectSelector.GetComponent<ObjectSelector>().selectedObject;

        if (SelectedBlock == null)
        {
            Debug.Log("No selected block");
            return;
        }

        SetBlockTypeAndInstantiate();
        SetBlockDirection();

        NewBlock.transform.position = SelectedBlock.transform.position;
        ObjectSelector.GetComponent<ObjectSelector>().selectedObject = NewBlock;
        Destroy(SelectedBlock);
    }

    void DeleteBlock()
    {
        GameObject SelectedBlock = ObjectSelector.GetComponent<ObjectSelector>().selectedObject;

        if (SelectedBlock == null)
        {
            Debug.Log("No selected block. Delete failed.");
            return;
        }

        Destroy(SelectedBlock);
    }

    private void SetBlockTypeAndInstantiate()
    {
        GameObject[] notePrefabs = { WholeNote, HalfNote, QuarterNote, EighthNote, SixteenthNote };
        NoteBlock.NoteLength[] noteLengths =
        {
            NoteBlock.NoteLength.Whole,
            NoteBlock.NoteLength.Half,
            NoteBlock.NoteLength.Quarter,
            NoteBlock.NoteLength.Eighth,
            NoteBlock.NoteLength.Sixteenth
        };
        int durationIndex = DurationDropdown.value;

        NewBlock = Instantiate(notePrefabs[durationIndex], Vector3.zero, Quaternion.identity);
        NewBlock.GetComponent<NoteBlock>().noteLength = noteLengths[durationIndex];
    }

    private void SetBlockDirection()
    {
        float[] angles = { 0f, 90f, 180f, -90f };
        NoteBlock.Direction[] directions =
        {
            NoteBlock.Direction.Right,
            NoteBlock.Direction.Up,
            NoteBlock.Direction.Left,
            NoteBlock.Direction.Down
        };

        int directionIndex = DirectionDropdown.value;

        NewBlock.transform.rotation = Quaternion.Euler(0, 0, angles[directionIndex]);
        NewBlock.GetComponent<NoteBlock>().direction = directions[directionIndex];
    }
}
