using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MapEditor : MonoBehaviour
{
    GameObject NewBlock;
    [SerializeField] GameObject DottedWholeNote;
    [SerializeField] GameObject WholeNote;
    [SerializeField] GameObject DottedHalfNote;
    [SerializeField] GameObject HalfNote;
    [SerializeField] GameObject DottedQuarterNote;
    [SerializeField] GameObject QuarterNote;
    [SerializeField] GameObject DottedEighthNote;
    [SerializeField] GameObject EighthNote;
    [SerializeField] GameObject DottedSixteenthNote;
    [SerializeField] GameObject SixteenthNote;

    [SerializeField] private Button createButton;
    [SerializeField] private Button changeButton;
    [SerializeField] private Button deleteButton;
    private Button saveButton;

    [SerializeField] TMP_Dropdown DurationDropdown;
    [SerializeField] TMP_Dropdown DirectionDropdown;

    [SerializeField] GameObject ObjectSelector;
    Dictionary<string, GameObject> notePrefabs;
    Dictionary<string, int> noteDirections;
    private int recentNoteIndex;

    void Start()
    {
        InitButtonEvent();
        InitNoteVariables();
        recentNoteIndex = 0;
    }

    void InitButtonEvent()
    {
        createButton.onClick.AddListener(CreateNewBlock);
        changeButton.onClick.AddListener(ChangeBlock);
        deleteButton.onClick.AddListener(DeleteBlock);
    }

    void InitNoteVariables()
    {
        notePrefabs = new Dictionary<string, GameObject>
        {
            { "DottedWholeNote", DottedWholeNote },
            { "WholeNote", WholeNote },
            { "DottedHalfNote", DottedHalfNote },
            { "HalfNote", HalfNote },
            { "DottedQuarterNote", DottedQuarterNote },
            { "QuarterNote", QuarterNote },
            { "DottedEighthNote", DottedEighthNote },
            { "EighthNote", EighthNote },
            { "DottedSixteenthNote", DottedSixteenthNote },
            { "SixteenthNote", SixteenthNote }
        };

        noteDirections = new Dictionary<string, int>
        {
            { "Up", 90 },
            { "Down", -90 },
            { "Right", 0 },
            { "Left", 180 },
            { "UpRight", 45 },
            { "UpLeft", 135 },
            { "DownRight", -45 },
            { "DownLeft", -135 }
        };
    }

    void CreateNewBlock()
    {
        SetBlockTypeAndInstantiate();
        SetBlockDirection();
        NewBlock.GetComponent<NoteBlock>().order = recentNoteIndex++;
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
        string duration = DurationDropdown.options[DurationDropdown.value].text;

        NewBlock = Instantiate(notePrefabs[duration], Vector3.zero, Quaternion.identity);
        NewBlock.GetComponent<NoteBlock>().noteLength = duration;
    }

    private void SetBlockDirection()
    {
        string direction = DirectionDropdown.options[DirectionDropdown.value].text;

        NewBlock.transform.rotation = Quaternion.Euler(0, 0, noteDirections[direction]);
        NewBlock.GetComponent<NoteBlock>().direction = direction;
    }
}
