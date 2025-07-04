using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] string mapName;
    [SerializeField] int nextMapIndex;
    [SerializeField] GameObject MenuObject;
    [SerializeField] GameObject ending;
    [SerializeField] Bar_Judge_Movement player;
    private int currentSceneIndex = 0;

    protected override void Awake()
    {
        base.Awake(); // 싱글톤 초기화 호출
        Debug.Log("GameManager Initialized");
    }

    void Update()
    {
        // if (Input.GetMouseButtonDown(1))
        // {
        //     player.GameOver(); // 디버그용 게임 오버
        // }

        if (Input.GetKeyDown(KeyCode.Escape) && MenuObject != null)
        {
            if (MenuObject.activeSelf)
            {
                MenuObject.SetActive(false);
                player.ResumeGame();
            }
            else
            {
                MenuObject.SetActive(true);
                player.PauseGame();
            }
        }
    }

    public void LoadNextMap()
    {
        if (nextMapIndex < SceneManager.sceneCountInBuildSettings)
        {
            Invoke("LoadNextMapScene", 3.0f);
        }
        else
        {
            Debug.Log("마지막 스테이지입니다!");
            ending.SetActive(true);
        }
    }

    private void LoadNextMapScene()
    {
        SceneManager.LoadScene(nextMapIndex);
    }

    public void QuitGame()
    {
        UnityEngine.Application.Quit();
    }
}
