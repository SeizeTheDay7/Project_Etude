using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SFB;
using System.IO;
using System;
using Unity.VisualScripting;

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
    private List<NoteBlockData> noteBlockDataList;
    [SerializeField] GameObject MapRoot;
    private Vector3 spawnPosition;
    private string selectedFilePath;
    private int NoteAllocateIndex;
    [SerializeField] GameObject TestBlock;

    void Start()
    {
        InitButtonEvent();
        InitNoteVariables();
        NoteAllocateIndex = 0;
    }

    void InitButtonEvent()
    {
        NoteCreateButton.onClick.AddListener(CreateNewBlock);
        NoteChangeButton.onClick.AddListener(ChangeBlock);
        NoteDeleteButton.onClick.AddListener(DeleteBlock);
        MapLoadButton.onClick.AddListener(LoadMap);
        MapSaveButton.onClick.AddListener(SaveMap);
        MapAddButton.onClick.AddListener(AddMap);
    }

    void InitNoteVariables()
    {
        notePrefabs = new Dictionary<string, GameObject>
        {
            { "DottedWhole", DottedWholeNote },
            { "Whole", WholeNote },
            { "DottedHalf", DottedHalfNote },
            { "Half", HalfNote },
            { "DottedQuarter", DottedQuarterNote },
            { "Quarter", QuarterNote },
            { "DottedEighth", DottedEighthNote },
            { "Eighth", EighthNote },
            { "DottedSixteenth", DottedSixteenthNote },
            { "Sixteenth", SixteenthNote }
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
        noteBlockDataList = new List<NoteBlockData>();
    }

    void CreateNewBlock()
    {
        InstantiateBlock();
        AddNewBlockDataToList(); // 새 블럭 정보 저장용 리스트에 추가
        NewBlock.GetComponent<NoteBlockIndex>().noteBlockIndex = NoteAllocateIndex++; // 접근용 인덱스 할당
    }

    void ChangeBlock()
    {
        GameObject SelectedBlock = ObjectSelector.GetComponent<ObjectSelector>().selectedObject;

        if (SelectedBlock == null)
        {
            Debug.Log("No selected block. Change failed.");
            return;
        }

        // 새 블럭 생성 및 정보 복사 → 리스트 정보, 선택 블럭 갱신 → 기존 블럭 삭제
        InstantiateBlock();
        NewBlock.GetComponent<NoteBlockIndex>().noteBlockIndex = SelectedBlock.GetComponent<NoteBlockIndex>().noteBlockIndex;
        NewBlock.transform.position = SelectedBlock.transform.position;

        UpdateBlockDataInList();
        ObjectSelector.GetComponent<ObjectSelector>().selectedObject = NewBlock;

        Destroy(SelectedBlock);

        // TODO:: 이후 순서인 모든 블럭의 위치를 재배치
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

        // TODO:: 이후의 모든 블럭의 위치를 재배치하고 인덱스를 재할당
    }

    private void InstantiateBlock()
    {
        string duration = DurationDropdown.options[DurationDropdown.value].text;
        string direction = DirectionDropdown.options[DirectionDropdown.value].text;

        NewBlock = Instantiate(notePrefabs[duration], spawnPosition, Quaternion.identity);
        NewBlock.transform.rotation = Quaternion.Euler(0, 0, noteDirections[direction]);
        NewBlock.transform.SetParent(MapRoot.transform);
        spawnPosition += GetDisplacement(new NoteBlockData { noteLength = duration, direction = direction });
    }

    /// <summary>
    /// 새로 만든 블럭 오브젝트의 정보를 리스트에 저장
    /// </summary>
    private void AddNewBlockDataToList()
    {
        NoteBlockData NewBlockData = new NoteBlockData();
        NewBlockData.noteLength = DurationDropdown.options[DurationDropdown.value].text;
        NewBlockData.direction = DirectionDropdown.options[DirectionDropdown.value].text;
        NewBlockData.order = NoteAllocateIndex;
        noteBlockDataList.Add(NewBlockData);
    }

    /// <summary>
    /// 편집된 블럭의 정보를 리스트에서 갱신
    /// </summary>
    private void UpdateBlockDataInList()
    {
        NoteBlockData UpdatedBlockData = new NoteBlockData();
        UpdatedBlockData.noteLength = DurationDropdown.options[DurationDropdown.value].text;
        UpdatedBlockData.direction = DirectionDropdown.options[DirectionDropdown.value].text;
        UpdatedBlockData.order = NewBlock.GetComponent<NoteBlockIndex>().noteBlockIndex;
        noteBlockDataList[NewBlock.GetComponent<NoteBlockIndex>().noteBlockIndex] = UpdatedBlockData;
    }

    private void LoadMap()
    {
        string filePath = StandaloneFileBrowser.OpenFilePanel("Select a Map", Application.persistentDataPath, "etude", false)[0];
        List<NoteBlockData> NoteBlockDataList = null;

        if (filePath.Length > 0 && !string.IsNullOrEmpty(filePath))
        {
            selectedFilePath = filePath;
            Debug.Log("Selected File: " + selectedFilePath);

            // Easy Save로 선택한 파일 로드
            if (ES3.FileExists(selectedFilePath))
            {
                NoteBlockDataList = ES3.Load<List<NoteBlockData>>("Map", selectedFilePath);
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
        spawnPosition = Vector3.zero; // 다음 스폰 위치
        foreach (NoteBlockData noteBlockData in NoteBlockDataList)
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
        Vector3 displacement_vector = Vector3.zero;
        float displacement = 0;

        switch (noteBlockData.noteLength)
        {
            case "DottedWhole":
                displacement = 6.0f; // 4.0f * 1.5
                break;
            case "Whole":
                displacement = 4.0f;
                break;
            case "DottedHalf":
                displacement = 3.0f; // 2.0f * 1.5
                break;
            case "Half":
                displacement = 2.0f;
                break;
            case "DottedQuarter":
                displacement = 1.5f; // 1.0f * 1.5
                break;
            case "Quarter":
                displacement = 1.0f;
                break;
            case "DottedEighth":
                displacement = 0.75f; // 0.5f * 1.5
                break;
            case "Eighth":
                displacement = 0.5f;
                break;
            case "DottedSixteenth":
                displacement = 0.375f; // 0.25f * 1.5
                break;
            case "Sixteenth":
                displacement = 0.25f;
                break;
        }

        switch (noteBlockData.direction)
        {
            case "Up":
                displacement_vector = new Vector3(0, displacement, 0);
                break;
            case "Down":
                displacement_vector = new Vector3(0, -displacement, 0);
                break;
            case "Right":
                displacement_vector = new Vector3(displacement, 0, 0);
                break;
            case "Left":
                displacement_vector = new Vector3(-displacement, 0, 0);
                break;
            case "UpRight":
                displacement_vector = new Vector3(displacement * Mathf.Sqrt(2) / 2, displacement * Mathf.Sqrt(2) / 2, 0);
                break;
            case "UpLeft":
                displacement_vector = new Vector3(-displacement * Mathf.Sqrt(2) / 2, displacement * Mathf.Sqrt(2) / 2, 0);
                break;
            case "DownRight":
                displacement_vector = new Vector3(displacement * Mathf.Sqrt(2) / 2, -displacement * Mathf.Sqrt(2) / 2, 0);
                break;
            case "DownLeft":
                displacement_vector = new Vector3(-displacement * Mathf.Sqrt(2) / 2, -displacement * Mathf.Sqrt(2) / 2, 0);
                break;
        }

        return displacement_vector;
    }

    private void SaveMap()
    {
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

    /// <summary>
    /// 경로만 설정해주고, 나머지는 easy save에게 맡긴다
    /// </summary>
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


