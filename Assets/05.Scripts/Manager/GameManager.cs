using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] string mapName;
    [SerializeField] Bar_Judge_Movement player;

    protected override void Awake()
    {
        base.Awake(); // 싱글톤 초기화 호출
        Debug.Log("GameManager Initialized");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            player.GameOver(); // 디버그용 게임 오버
        }
    }
}
