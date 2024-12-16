// using System;
// using System.IO;
// using UnityEditor;
// using UnityEngine;

// [System.Serializable]
// public class test
// {
//     public int testvar;
// }

// public class JSONtest : MonoBehaviour
// {
//     private test testdata;
//     private string filePath;

//     // Start is called before the first frame update
//     void Start()
//     {
//         filePath = Path.Combine(Application.persistentDataPath, "playerData.json"); // 경로랑 파일 이름 지정하고
//         testdata = new test { testvar = 10 }; // 직렬화된 클래스 데이터 만들고
//         string json = JsonUtility.ToJson(testdata); // JSON으로 만들고
//         File.WriteAllText(filePath, json); // 파일에 쓰고
//         Debug.Log(json);
//     }
// }