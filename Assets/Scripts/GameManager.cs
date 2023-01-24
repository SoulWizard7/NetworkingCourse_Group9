using System;
using System.Collections;
using System.Collections.Generic;
using Alteruna;
using Alteruna.Trinity;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [SerializeField] private Multiplayer multiplayer;
    [SerializeField] private Spawner _spawner;

    [SerializeField]
    private bool gameStarted = false;

    [SerializeField] private float asteroidCDTime = 10f;
    
    public bool activeAsteroid = false;
    private float asteroidTimeSinceDeath = 0;

    private GameInstance _gameInstance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        _gameInstance = GameObject.Find("GameInstance").GetComponent<GameInstance>();
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

    public void SpawnAsteroid()
    {
        _spawner.Spawn(2, new Vector3(0, 2, 0));
        activeAsteroid = true;
    }

    private void Update()
    {
        if (_gameInstance.GameStateInfo.State != GameState.GAME_RUNNING) return; 
        
        if (!activeAsteroid)
        {
            asteroidTimeSinceDeath += Time.deltaTime;
            if (asteroidTimeSinceDeath >= asteroidCDTime)
            {
                SpawnAsteroid();
                asteroidTimeSinceDeath = 0;
                activeAsteroid = true;
            }
        }
    }
}
