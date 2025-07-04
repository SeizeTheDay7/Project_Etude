using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SFB;
using System.IO;
using System;
using Unity.VisualScripting;
using System.Linq;

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

/// <summary>
/// 파일에 저장할 노트 블럭 정보들
/// </summary>
public class NoteBlockData
{
    public string noteName;
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
    [SerializeField] GameObject EndNote;

    [Header("UI Elements")]
    [SerializeField] private Button NoteCreateButton;
    [SerializeField] private Button NoteChangeButton;
    [SerializeField] private Button NoteDeleteButton;

    [SerializeField] private Button MapLoadButton;
    [SerializeField] private Button MapSaveButton;
    [SerializeField] private Button MapAddButton;
    [SerializeField] private TMP_InputField FileNameInputField;

    [SerializeField] TMP_Dropdown NoteNameDropdown;
    [SerializeField] TMP_Dropdown NoteDirectionDropdown;

    [Header("Map Elements")]
    [SerializeField] GameObject ObjectSelector;
    [SerializeField] GameObject MapRoot;
    Dictionary<string, GameObject> notePrefabs;
    Dictionary<string, float> noteDuration;
    Dictionary<string, int> noteDirections;
    private string selectedFilePath;
    private List<NoteBlockData> noteBlockDataList; // 노트 블럭 정보 저장용 리스트
    private Vector3 spawnPosition; // 블럭 생성 위치    
    private int NoteAllocateIndex; // noteBlockDataList 접근용 인덱스
    private GameObject NewBlock; // 새로 생성한 블럭
    private GameObject LastBlock; // 마지막으로 생성한 블럭

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
            { "Sixteenth", SixteenthNote },
            { "End", EndNote }
        };

        noteDuration = new Dictionary<string, float>
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
            { "Sixteenth", 0.25f },
            { "End", 0.0f }
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
            { "DownLeft", -135 },
            { "End", 0 }
        };

        spawnPosition = Vector3.zero;
        noteBlockDataList = new List<NoteBlockData>();
    }

    void CreateNewBlock()
    {
        InstantiateBlockAndStoreInformation();
        AddNewBlockDataToList(); // 새 블럭 정보 저장용 리스트에 추가
        ConnectBlocksAndUpdateLastBlock();
    }

    void ChangeBlock()
    {
        GameObject SelectedBlock = ObjectSelector.GetComponent<ObjectSelector>().selectedObject;

        if (SelectedBlock == null)
        {
            Debug.Log("No selected block. Change failed.");
            return;
        }
        int targetIndex = SelectedBlock.GetComponent<NoteBlock>().noteBlockIndex;
        UpdateBlockDataInList(targetIndex); // 리스트 정보 갱신

        // 선택된 블럭과 이후 블럭 모두 삭제 후 새로 생성
        WipeBlocksFromTheBlock(SelectedBlock);
        RegenerateBlocksFromTheIndex(targetIndex);
    }

    void DeleteBlock()
    {
        GameObject SelectedBlock = ObjectSelector.GetComponent<ObjectSelector>().selectedObject;

        if (SelectedBlock == null)
        {
            Debug.Log("No selected block. Delete failed.");
            return;
        }

        // 해당 블럭과 이후의 모든 블럭 삭제
        WipeBlocksFromTheBlock(SelectedBlock);
        // 리스트에서 해당 블럭 이후의 모든 블럭 정보 삭제
        if (NoteAllocateIndex == 0) noteBlockDataList.RemoveRange(0, noteBlockDataList.Count);
        else noteBlockDataList.RemoveRange(NoteAllocateIndex, noteBlockDataList.Count - NoteAllocateIndex);
    }

    /// <summary>
    /// 드롭다운에서 선택한 노트의 길이와 방향에 따라 블럭을 생성하고 정보를 기록
    /// </summary>
    private void InstantiateBlockAndStoreInformation()
    {
        string name = NoteNameDropdown.options[NoteNameDropdown.value].text;
        string direction = NoteDirectionDropdown.options[NoteDirectionDropdown.value].text;

        NewBlock = Instantiate(notePrefabs[name], spawnPosition, Quaternion.identity);
        NewBlock.transform.rotation = Quaternion.Euler(0, 0, noteDirections[direction]);
        NewBlock.transform.SetParent(MapRoot.transform);

        NoteBlock newBlockScript = NewBlock.GetComponent<NoteBlock>();
        newBlockScript.requiredKeys = (KeyType)Enum.Parse(typeof(KeyType), direction);
        newBlockScript.noteDuration = noteDuration[name];
        newBlockScript.noteBlockIndex = NoteAllocateIndex; // 접근용 인덱스 할당

        spawnPosition += GetDisplacement(name, direction);
    }

    private void DuplicateBlockData(GameObject SelectedBlock)
    {
        NoteBlock selectedScript = SelectedBlock.GetComponent<NoteBlock>();
        NoteBlock newBlockScript = NewBlock.GetComponent<NoteBlock>();
        newBlockScript.noteBlockIndex = selectedScript.noteBlockIndex;
        newBlockScript.prevNoteBlock = selectedScript.prevNoteBlock;
        newBlockScript.nextNoteBlock = selectedScript.nextNoteBlock;
        NewBlock.transform.position = SelectedBlock.transform.position;
    }

    private void ConnectBlocksAndUpdateLastBlock()
    {
        // 이전 블럭과 쌍방 연결
        if (NoteAllocateIndex != 0)
        {
            NewBlock.GetComponent<NoteBlock>().prevNoteBlock = LastBlock;
            LastBlock.GetComponent<NoteBlock>().nextNoteBlock = NewBlock; // 이새끼잘못
        }
        LastBlock = NewBlock;
        NoteAllocateIndex++;
    }

    /// <summary>
    /// 새로 만든 블럭 오브젝트의 정보를 리스트에 저장
    /// </summary>
    private void AddNewBlockDataToList()
    {
        NoteBlockData NewBlockData = new NoteBlockData();
        NewBlockData.noteName = NoteNameDropdown.options[NoteNameDropdown.value].text;
        NewBlockData.direction = NoteDirectionDropdown.options[NoteDirectionDropdown.value].text;
        NewBlockData.order = NoteAllocateIndex;
        noteBlockDataList.Add(NewBlockData);
    }

    /// <summary>
    /// 편집된 블럭의 정보를 리스트에서 갱신
    /// </summary>
    private void UpdateBlockDataInList(int targetIndex)
    {
        NoteBlockData UpdatedBlockData = new NoteBlockData();
        UpdatedBlockData.noteName = NoteNameDropdown.options[NoteNameDropdown.value].text;
        UpdatedBlockData.direction = NoteDirectionDropdown.options[NoteDirectionDropdown.value].text;
        UpdatedBlockData.order = targetIndex;
        noteBlockDataList[targetIndex] = UpdatedBlockData;
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

    private Vector3 GetDisplacement(string noteName, string direction)
    {
        Dictionary<string, Vector3> directionFormula = new Dictionary<string, Vector3>
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

        float displacement = noteDuration[noteName];
        Vector3 directionVector = directionFormula[direction];

        // Debug.Log("GetDisplacement :: NoteAllocateIndex :" + NoteAllocateIndex);

        return directionVector * displacement + new Vector3(0, 0, 0.000001f * (NoteAllocateIndex + 1));
    }

    private void LoadMap()
    {
        string[] filePaths = StandaloneFileBrowser.OpenFilePanel("Select a Map", Application.persistentDataPath, "etude", false);
        if (filePaths.Count() == 0)
        {
            Debug.LogWarning("No file selected.");
            return;
        }
        string filePath = filePaths[0];

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
                return;
            }
        }
        else
        {
            Debug.LogWarning("No file selected.");
            return;
        }

        // 기존 블럭들 삭제
        foreach (Transform child in MapRoot.transform)
        {
            Destroy(child.gameObject);
        }

        // noteBlockDataList에 있는 데이터로 MapRoot 아래에 노트 블럭들 생성
        RegenerateBlocksFromTheIndex(0);
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
        if (FileNameInputField.text == "")
        {
            Debug.LogWarning("No file name entered.");
            return;
        }
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

    private void RegenerateBlocksFromTheIndex(int index)
    {
        if (index == 0) spawnPosition = Vector3.zero; // 첫 블럭이면 위치 초기화
        NoteAllocateIndex = index; // 시작 인덱스 설정

        for (int i = index; i < noteBlockDataList.Count; i++)
        {
            NoteNameDropdown.value = NoteNameDropdown.options.FindIndex(option => option.text == noteBlockDataList[i].noteName);
            NoteDirectionDropdown.value = NoteDirectionDropdown.options.FindIndex(option => option.text == noteBlockDataList[i].direction);
            InstantiateBlockAndStoreInformation();
            ConnectBlocksAndUpdateLastBlock();
        }
    }
}


