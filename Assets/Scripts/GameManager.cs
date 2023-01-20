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

    [SerializeField] private float asteroidCDTime = 10f;
    float asteroidTimeSinceDeath = 0f;

    [SerializeField]
    private bool gameStarted = false;

    public bool activeAsteroid = false;
    
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
<<<<<<< Updated upstream
=======


    public void SpawnAsteroid()
    {
        _spawner.Spawn(2, new Vector3(0, 2, 0));
        activeAsteroid = true;
    }

    private void Update()
    {
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
>>>>>>> Stashed changes
}
