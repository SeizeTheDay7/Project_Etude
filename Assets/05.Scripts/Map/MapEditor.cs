using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SFB;
using System.IO;
using ES3Internal;
using System;

[System.Serializable]
public class NoteBlockData
{
    public string noteLength;
    public string direction;
    public int order;
}

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
    [SerializeField] private Button MapAddButton;

    [SerializeField] TMP_Dropdown DurationDropdown;
    [SerializeField] TMP_Dropdown DirectionDropdown;

    [SerializeField] GameObject ObjectSelector;
    Dictionary<string, GameObject> notePrefabs;
    Dictionary<string, int> noteDirections;
    private int recentNoteIndex;
    private List<NoteBlockData> noteBlockDataList;
    [SerializeField] GameObject MapRoot;
    private Vector3 spawnPosition;
    private string selectedFilePath;

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
        MapAddButton.onClick.AddListener(AddMap);
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

        spawnPosition = Vector3.zero;
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
        string filePath = StandaloneFileBrowser.OpenFilePanel("Select a Map", Application.persistentDataPath, "etude", false)[0];

        if (filePath.Length > 0 && !string.IsNullOrEmpty(filePath))
        {
            selectedFilePath = filePath;
            Debug.Log("Selected File: " + selectedFilePath);

            // Easy Save로 선택한 파일 로드
            if (ES3.FileExists(selectedFilePath))
            {
                noteBlockDataList = ES3.Load<List<NoteBlockData>>("Map", selectedFilePath);
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

        // noteBlockDataList에 있는 데이터로 MapRoot 아래에 노트 블럭들 생성
        spawnPosition = Vector3.zero;
        foreach (NoteBlockData noteBlockData in noteBlockDataList)
        {
            GameObject noteBlock = Instantiate(notePrefabs[noteBlockData.noteLength], spawnPosition, Quaternion.identity);
            noteBlock.transform.rotation = Quaternion.Euler(0, 0, noteDirections[noteBlockData.direction]);
            noteBlock.GetComponent<NoteBlockData>().noteLength = noteBlockData.noteLength;
            noteBlock.GetComponent<NoteBlockData>().direction = noteBlockData.direction;
            noteBlock.GetComponent<NoteBlockData>().order = noteBlockData.order;
            noteBlock.transform.SetParent(MapRoot.transform);
            spawnPosition += GetDisplacement(noteBlockData);
        }
    }

    private Vector3 GetDisplacement(NoteBlockData noteBlockData)
    {
        Vector3 spawnPosition = Vector3.zero;
        float displacement = 0;

        switch (noteBlockData.noteLength)
        {
            case "DottedWholeNote":
                displacement = 6.0f; // 4.0f * 1.5
                break;
            case "WholeNote":
                displacement = 4.0f;
                break;
            case "DottedHalfNote":
                displacement = 3.0f; // 2.0f * 1.5
                break;
            case "HalfNote":
                displacement = 2.0f;
                break;
            case "DottedQuarterNote":
                displacement = 1.5f; // 1.0f * 1.5
                break;
            case "QuarterNote":
                displacement = 1.0f;
                break;
            case "DottedEighthNote":
                displacement = 0.75f; // 0.5f * 1.5
                break;
            case "EighthNote":
                displacement = 0.5f;
                break;
            case "DottedSixteenthNote":
                displacement = 0.375f; // 0.25f * 1.5
                break;
            case "SixteenthNote":
                displacement = 0.25f;
                break;
        }

        switch (noteBlockData.direction)
        {
            case "Up":
                spawnPosition = new Vector3(0, displacement, 0);
                break;
            case "Down":
                spawnPosition = new Vector3(0, -displacement, 0);
                break;
            case "Right":
                spawnPosition = new Vector3(displacement, 0, 0);
                break;
            case "Left":
                spawnPosition = new Vector3(-displacement, 0, 0);
                break;
            case "UpRight":
                spawnPosition = new Vector3(displacement * Mathf.Sqrt(2) / 2, displacement * Mathf.Sqrt(2) / 2, 0);
                break;
            case "UpLeft":
                spawnPosition = new Vector3(-displacement * Mathf.Sqrt(2) / 2, displacement * Mathf.Sqrt(2) / 2, 0);
                break;
            case "DownRight":
                spawnPosition = new Vector3(displacement * Mathf.Sqrt(2) / 2, -displacement * Mathf.Sqrt(2) / 2, 0);
                break;
            case "DownLeft":
                spawnPosition = new Vector3(-displacement * Mathf.Sqrt(2) / 2, -displacement * Mathf.Sqrt(2) / 2, 0);
                break;
        }

        return spawnPosition;
    }

    private void SaveMapData()
    {
        noteBlockDataList = new List<NoteBlockData>();
        // 테스트용 데이터 생성
        noteBlockDataList.Add(new NoteBlockData { noteLength = "WholeNote", direction = "Up", order = 0 });

        // noteBlockDataList에 있는 데이터를 easy save로 저장
        if (selectedFilePath != null)
        {
            Debug.Log("Will Save Data to: " + selectedFilePath);
            ES3.Save<List<NoteBlockData>>("Map", noteBlockDataList, selectedFilePath);
            Debug.Log("Data saved successfully.");
        }
        else
        {
            Debug.LogWarning("No file selected to save.");
        }
    }

    private void AddMap()
    {
        // TODO: fileName은 별도 UI로 입력받아야 함
        string fileName = "NewMap.etude";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        if (!File.Exists(filePath)) // 같은 파일 있는지 확인하고
        {
            selectedFilePath = filePath;
            Debug.Log($"File created: {selectedFilePath}");
        }
        else
        {
            Debug.LogWarning($"File already exists: {filePath}");
        }
    }
}


