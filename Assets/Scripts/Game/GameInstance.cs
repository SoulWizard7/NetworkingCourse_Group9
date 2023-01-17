using Alteruna;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum GameState
{
    GAME_PAUSED,
    GAME_RUNNING,
    GAME_STOPPED,
    GAME_WAITING,
    GAME_WAITING_TO_START
}

public class GameInstance : MonoBehaviour
{
    public List<User> Users { get => _multiplayer.GetUsers(); }

    public GameState State = GameState.GAME_WAITING;
    public float TimeUntilGameStart = 5.0f;

    public Timer _timer;

    private Multiplayer _multiplayer;
    private TextChatSynchronizable _textChat;
    private UIManager _uiManager;

    private void Awake()
    {
        _uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        _uiManager.onNameEntered += _uiManager_onNameEntered;
    }

    private void _uiManager_onNameEntered(Multiplayer multiplayer)
    {
        Setup(multiplayer);
    }

    private void Setup(Multiplayer multiplayer)
    {
        _multiplayer = multiplayer;
        //_textChat = GameObject.Find("Text Chat").GetComponent<TextChatSynchronizable>();
        _multiplayer.OtherUserJoined.AddListener(HandleUserJoined);
        Debug.LogWarning("SETUP!");
    }

    void HandleUserJoined(Multiplayer multiplayer, User user)
    {
        if(multiplayer.GetUsers().Count == multiplayer.MaxPlayers)
        {
            TryStartGame();
        }
        if(multiplayer.GetUsers().Count != multiplayer.MaxPlayers)
        {
            AbortGameStart();
        }
        Debug.Log($"{multiplayer.GetUsers().Count} out of {multiplayer.MaxPlayers} has joined!");
    }

    private void TryStartGame()
    {
        State = GameState.GAME_WAITING_TO_START;
        _timer = new Timer(StartGame, null, (int)TimeUntilGameStart, 0);
    }

    private void AbortGameStart()
    {
        State = GameState.GAME_STOPPED;
        _timer?.Dispose();
    }

    private void StartGame(object state)
    {
        State = GameState.GAME_RUNNING;
        _timer?.Dispose();
    }
}
