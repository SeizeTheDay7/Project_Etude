using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SFB;
using System.IO;
using System;

[System.Flags]
public enum KeyType
{
    None = 0,
    Up = 1 << 0,
    Down = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3,
    UpRight = Up | Right,
    UpLeft = Up | Left,
    DownRight = Down | Right,
    DownLeft = Down | Left
}

public class NoteBlockData
{
    public string noteLength;
    public string direction;
    public int order;
}

public class MapEditor : MonoBehaviour
{
    // [SerializeField] MapLoader mapLoader;

    [Header("Note Prefabs")]
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

    [Header("UI Elements")]
    [SerializeField] private Button NoteCreateButton;
    [SerializeField] private Button NoteChangeButton;
    [SerializeField] private Button NoteDeleteButton;

    [SerializeField] private Button MapLoadButton;
    [SerializeField] private Button MapSaveButton;
    [SerializeField] private Button MapAddButton;
    [SerializeField] private InputField FileNameInputField;

    [SerializeField] TMP_Dropdown DurationDropdown;
    [SerializeField] TMP_Dropdown DirectionDropdown;

    [Header("Map Elements")]
    [SerializeField] GameObject ObjectSelector;
    [SerializeField] GameObject MapRoot;
    Dictionary<string, GameObject> notePrefabs;
    Dictionary<string, int> noteDirections;
    private string selectedFilePath;
    private List<NoteBlockData> noteBlockDataList; // 노트 블럭 정보 저장용 리스트
    private Vector3 spawnPosition; // 블럭 생성 위치    
    private int NoteAllocateIndex; // noteBlockDataList 접근용 인덱스
    private GameObject NewBlock; // 새로 생성한 블럭
    private GameObject LastBlock; // 가장 최근 생성한 블럭

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
        NewBlock.GetComponent<NoteBlock>().requiredKeys = (KeyType)Enum.Parse(typeof(KeyType), DirectionDropdown.options[DirectionDropdown.value].text);
        // NewBlock.GetComponent<SortingGroup>().sortingOrder = -NoteAllocateIndex; // 보이는 우선순위 할당
        NewBlock.GetComponent<NoteBlock>().noteBlockIndex = NoteAllocateIndex; // 접근용 인덱스 할당

        // 이전 블럭과 쌍방 연결
        if (NoteAllocateIndex != 0)
        {
            NewBlock.GetComponent<NoteBlock>().prevNoteBlock = LastBlock;
            LastBlock.GetComponent<NoteBlock>().nextNoteBlock = NewBlock;
        }
        LastBlock = NewBlock;
        NoteAllocateIndex++;
    }

    void ChangeBlock()
    {
        // TODO: requiredKeys 갱신
        GameObject SelectedBlock = ObjectSelector.GetComponent<ObjectSelector>().selectedObject;

        if (SelectedBlock == null)
        {
            Debug.Log("No selected block. Change failed.");
            return;
        }

        // 새 블럭 생성 및 정보 복사
        InstantiateBlock();
        var selectedBlockIndex = SelectedBlock.GetComponent<NoteBlock>();
        var newBlockIndex = NewBlock.GetComponent<NoteBlock>();
        newBlockIndex.noteBlockIndex = selectedBlockIndex.noteBlockIndex;
        newBlockIndex.prevNoteBlock = selectedBlockIndex.prevNoteBlock;
        newBlockIndex.nextNoteBlock = selectedBlockIndex.nextNoteBlock;
        NewBlock.transform.position = SelectedBlock.transform.position;

        // 리스트 정보, 선택 블럭 갱신
        UpdateBlockDataInList();
        ObjectSelector.GetComponent<ObjectSelector>().selectedObject = NewBlock;

        // 이후 블럭 모두 삭제
        WipeBlocksFromTheBlock(NewBlock.GetComponent<NoteBlock>().nextNoteBlock);
    }

    void DeleteBlock()
    {
        GameObject SelectedBlock = ObjectSelector.GetComponent<ObjectSelector>().selectedObject;

        if (SelectedBlock == null)
        {
            Debug.Log("No selected block. Delete failed.");
            return;
        }

        // 이전 블럭과 다음 블럭 연결
        var selectedBlockIndex = SelectedBlock.GetComponent<NoteBlock>();
        if (selectedBlockIndex.prevNoteBlock != null)
        {
            selectedBlockIndex.prevNoteBlock.GetComponent<NoteBlock>().nextNoteBlock = selectedBlockIndex.nextNoteBlock;
        }

        // 해당 블럭과 이후의 모든 블럭 삭제
        WipeBlocksFromTheBlock(SelectedBlock);
    }

    /// <summary>
    /// 드롭다운에서 선택한 노트의 길이와 방향에 따라 블럭을 생성
    /// </summary>
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
        UpdatedBlockData.order = NewBlock.GetComponent<NoteBlock>().noteBlockIndex;
        noteBlockDataList[NewBlock.GetComponent<NoteBlock>().noteBlockIndex] = UpdatedBlockData;
    }

    /// <summary>
    /// 해당 블럭과 이후의 모든 블럭을 삭제
    /// </summary>
    private void WipeBlocksFromTheBlock(GameObject TheBlock)
    {
        LastBlock = TheBlock.GetComponent<NoteBlock>().prevNoteBlock;
        NoteAllocateIndex = TheBlock.GetComponent<NoteBlock>().noteBlockIndex;
        spawnPosition = TheBlock.transform.position;

        while (TheBlock != null)
        {
            GameObject temp = TheBlock.GetComponent<NoteBlock>().nextNoteBlock;
            Destroy(TheBlock);
            TheBlock = temp;
        }
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

        // 기존 블럭들 삭제
        foreach (Transform child in MapRoot.transform)
        {
            Destroy(child.gameObject);
        }

        // noteBlockDataList에 있는 데이터로 MapRoot 아래에 노트 블럭들 생성
        spawnPosition = Vector3.zero; // 다음 스폰 위치
        foreach (NoteBlockData noteBlockData in NoteBlockDataList)
        {
            GameObject noteBlock = Instantiate(notePrefabs[noteBlockData.noteLength], spawnPosition, Quaternion.identity);
            noteBlock.transform.rotation = Quaternion.Euler(0, 0, noteDirections[noteBlockData.direction]);
            noteBlock.GetComponent<NoteBlock>().noteBlockIndex = noteBlockData.order;
            noteBlock.transform.SetParent(MapRoot.transform);
            spawnPosition += GetDisplacement(noteBlockData);
        }
    }

    private Vector3 GetDisplacement(NoteBlockData noteBlockData)
    {
        Dictionary<string, float> noteLengths = new Dictionary<string, float>
        {
            { "DottedWhole", 6.0f },
            { "Whole", 4.0f },
            { "DottedHalf", 3.0f },
            { "Half", 2.0f },
            { "DottedQuarter", 1.5f },
            { "Quarter", 1.0f },
            { "DottedEighth", 0.75f },
            { "Eighth", 0.5f },
            { "DottedSixteenth", 0.375f },
            { "Sixteenth", 0.25f }
        };

        Dictionary<string, Vector3> directions = new Dictionary<string, Vector3>
        {
            { "Up", new Vector3(0, 1, 0) },
            { "Down", new Vector3(0, -1, 0) },
            { "Right", new Vector3(1, 0, 0) },
            { "Left", new Vector3(-1, 0, 0) },
            { "UpRight", new Vector3(Mathf.Sqrt(2) / 2, Mathf.Sqrt(2) / 2, 0) },
            { "UpLeft", new Vector3(-Mathf.Sqrt(2) / 2, Mathf.Sqrt(2) / 2, 0) },
            { "DownRight", new Vector3(Mathf.Sqrt(2) / 2, -Mathf.Sqrt(2) / 2, 0) },
            { "DownLeft", new Vector3(-Mathf.Sqrt(2) / 2, -Mathf.Sqrt(2) / 2, 0) }
        };

        float displacement = noteLengths[noteBlockData.noteLength];
        Vector3 directionVector = directions[noteBlockData.direction];

        // Debug.Log("GetDisplacement :: NoteAllocateIndex :" + NoteAllocateIndex);

        return directionVector * displacement + new Vector3(0, 0, 0.000001f * (NoteAllocateIndex + 1));
    }

    /// <summary>
    /// noteBlockDataList에 있는 데이터를 easy save로 저장
    /// </summary>
    private void SaveMap()
    {
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
    /// 파일 경로만 설정해주고, 나머지는 easy save에게 맡긴다
    /// </summary>
    private void AddMap()
    {
        // TODO: fileName은 별도 UI로 입력받아야 함
        string fileName = FileNameInputField.text + ".etude";
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


