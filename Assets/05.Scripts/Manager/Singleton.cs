// using UnityEngine;

// public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
// {
//     private static T instance;

//     public static T instance
//     {
//         get
//         {
//             if (Instance == null)
//             {
//                 Instance = FindObjectOfType<T>();
//             }
//             return Instance;
//         }
//     }

//     void Awake()
//     {
//         if (manager == null)
//         {
//             manager = this;
//             DontDestroyOnLoad(this);
//         }
//         else if (manager != this)
//         {
//             Destroy(gameObject);
//         }
//     }
// }