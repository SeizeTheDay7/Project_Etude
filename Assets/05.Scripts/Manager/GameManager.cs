using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] string mapName;

    protected override void Awake()
    {
        base.Awake(); // 싱글톤 초기화 호출
        Debug.Log("GameManager Initialized");
    }

    void Update()
    {

    }
}
