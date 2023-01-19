using System;
using System.Collections;
using System.Collections.Generic;
using Alteruna;
using Alteruna.Trinity;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [SerializeField] private Multiplayer multiplayer;
    [SerializeField] private Spawner _spawner;

    [SerializeField]
    private bool gameStarted = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public bool GameStarted
    {
        get => gameStarted;
        
        set { gameStarted = value; }
    }
    
    public void StartGame()
    {
        Debug.Log("Game Session Started");
        GameStarted = true;
    }
}
