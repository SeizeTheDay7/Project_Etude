using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SFB;
using ES3Internal;
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

    [SerializeField] private Button NoteCreateButton;
    [SerializeField] private Button NoteChangeButton;
    [SerializeField] private Button NoteDeleteButton;

    [SerializeField] private Button MapLoadButton;
    [SerializeField] private Button MapSaveButton;

    [SerializeField] TMP_Dropdown DurationDropdown;
    [SerializeField] TMP_Dropdown DirectionDropdown;

    [SerializeField] GameObject ObjectSelector;
    Dictionary<string, GameObject> notePrefabs;
    Dictionary<string, int> noteDirections;
    private int recentNoteIndex;
    private List<NoteBlockData> noteBlockDataList;
    private GameObject MapRoot;

    void Start()
    {
        InitButtonEvent();
        InitNoteVariables();
        recentNoteIndex = 0;
    }

    void InitButtonEvent()
    {
        NoteCreateButton.onClick.AddListener(CreateNewBlock);
        NoteChangeButton.onClick.AddListener(ChangeBlock);
        NoteDeleteButton.onClick.AddListener(DeleteBlock);
        MapLoadButton.onClick.AddListener(LoadMapData);
        MapSaveButton.onClick.AddListener(SaveMapData);
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
        NewBlock.GetComponent<NoteBlockData>().order = recentNoteIndex++;
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
        NewBlock.GetComponent<NoteBlockData>().noteLength = duration;
    }

    private void SetBlockDirection()
    {
        string direction = DirectionDropdown.options[DirectionDropdown.value].text;

        NewBlock.transform.rotation = Quaternion.Euler(0, 0, noteDirections[direction]);
        NewBlock.GetComponent<NoteBlockData>().direction = direction;
    }

    private void LoadMapData()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Select a Map", Application.persistentDataPath, "etude", false);

        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            string selectedFilePath = paths[0];
            Debug.Log("Selected File: " + selectedFilePath);

            // Easy Save로 선택한 파일 로드
            if (ES3.FileExists(selectedFilePath))
            {
                noteBlockDataList = ES3.Load<List<NoteBlockData>>("NoteBlocks", selectedFilePath);
                Debug.Log("Data loaded successfully.");
            }
            else
            {
                Debug.LogWarning("File not found or incompatible with Easy Save.");
            }
        }
        else
        {
            Debug.LogWarning("No file selected.");
        }

        // TODO: noteBlockDataList에 있는 데이터로 MapRoot 아래에 노트 블럭들 생성



    }

    private void SaveMapData()
    {
        // TODO: noteBlockDataList에 있는 데이터를 easy save로 저장

    }
}
