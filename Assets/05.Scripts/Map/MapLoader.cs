// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class MapLoader : MonoBehaviour
// {
//     // Start is called before the first frame update
//     void Start()
//     {

//     }

//     // Update is called once per frame
//     void Update()
//     {

//     }

//     public void LoadMap(string filePath)
//     {
//         if (filePath.Length > 0 && !string.IsNullOrEmpty(filePath))
//         {
//             selectedFilePath = filePath;
//             Debug.Log("Selected File: " + selectedFilePath);

//             // Easy Save로 선택한 파일 로드
//             if (ES3.FileExists(selectedFilePath))
//             {
//                 NoteBlockDataList = ES3.Load<List<NoteBlockData>>("Map", selectedFilePath);
//                 Debug.Log("Data loaded successfully.");
//             }
//             else
//             {
//                 Debug.LogWarning("File not found or incompatible with Easy Save.");
//             }
//         }
//         else
//         {
//             Debug.LogWarning("No file selected.");
//         }

//         // 기존 블럭들 삭제
//         foreach (Transform child in MapRoot.transform)
//         {
//             Destroy(child.gameObject);
//         }

//         // noteBlockDataList에 있는 데이터로 MapRoot 아래에 노트 블럭들 생성
//         spawnPosition = Vector3.zero; // 다음 스폰 위치
//         foreach (NoteBlockData noteBlockData in NoteBlockDataList)
//         {
//             GameObject noteBlock = Instantiate(notePrefabs[noteBlockData.noteLength], spawnPosition, Quaternion.identity);
//             noteBlock.transform.rotation = Quaternion.Euler(0, 0, noteDirections[noteBlockData.direction]);
//             noteBlock.GetComponent<NoteBlockIndex>().noteBlockIndex = noteBlockData.order;
//             noteBlock.transform.SetParent(MapRoot.transform);
//             spawnPosition += GetDisplacement(noteBlockData);
//         }
//     }
// }
