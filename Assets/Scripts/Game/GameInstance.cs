using Alteruna;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public enum GameState
{
    GAME_PAUSED,
    GAME_RUNNING,
    GAME_STOPPED,
    GAME_WAITING_FOR_PLAYERS,
    GAME_WAITING_TO_START
}

[RequireComponent(typeof(GameInstanceSynchronizable))]
public class GameInstance : MonoBehaviour
{
    [SerializeField] private Multiplayer MultiplayerPrefab;

    private List<SetupFinished> _setupFinished = new List<SetupFinished>();

    private TextChatSynchronizable _textChat;
    private GameInstanceSynchronizable _gameInstanceSynchronizable;

    public List<User> Users { get => Multiplayer.GetUsers(); }
    public delegate void SetupFinished();

    public event SetupFinished OnConnected
    {
        add => _setupFinished.Add(value);
        remove => _setupFinished.Remove(value);
    }

    public GameState State = GameState.GAME_WAITING_FOR_PLAYERS;
    public float TimeUntilGameStart = 5.0f;

    public Timer _timer;

    internal UnityEvent<GameState> GameStateChanged = new UnityEvent<GameState>();
    internal UnityEvent<Dictionary<ushort, int>> ScoresUpdated = new UnityEvent<Dictionary<ushort, int>>();
    internal Multiplayer Multiplayer;

    private void Awake()
    {
        _gameInstanceSynchronizable = GetComponent<GameInstanceSynchronizable>();
    }

    internal void Setup(string name, int playerCount)
    {
        Multiplayer = Instantiate(MultiplayerPrefab, Vector3.zero, Quaternion.identity);
        Multiplayer.SetUsername(name);
        Multiplayer.MaxPlayers = (ushort)playerCount;
        Multiplayer.Connected.AddListener(SetupMultiplayerListeners);
        Multiplayer.Disconnected.AddListener((mp, endpoint) => Debug.Log("DISCONNECTED"));
        _gameInstanceSynchronizable.enabled = true;
        _gameInstanceSynchronizable.GameStateChanged.AddListener(state => { State = state; GameStateChanged.Invoke(state); });
        _gameInstanceSynchronizable.ScoreChanged.AddListener(scores => ScoresUpdated.Invoke(scores));
        Debug.LogWarning("SETUP!");
    }

    void SetupMultiplayerListeners(Multiplayer multiplayer, Endpoint endpoint)
    {
        Debug.Log("CONNECTED!!");

        Multiplayer.OtherUserJoined.AddListener(HandleUserJoined);

        Multiplayer.RoomJoined.AddListener((multiplayer, room, user) =>
        {
            State = GameState.GAME_WAITING_FOR_PLAYERS;
            _timer?.Dispose();
            multiplayer.GetUsers().ForEach(u => Debug.Log(u.Name));
            _gameInstanceSynchronizable._scores.Add(user.Index, 0);
            GameStateChanged.Invoke(State);
        });

        Multiplayer.RoomLeft.AddListener(multiplayer =>
        {
            
        });

        Multiplayer.OtherUserLeft.AddListener(HandleUserLeft);

        _timer = new Timer(RefreshAvailableRooms, null, 0, 2000);
        _setupFinished.ForEach(e => e());
    }

    void HandleUserJoined(Multiplayer multiplayer, User user)
    {
        multiplayer.GetUsers().ForEach(u => Debug.Log(u.Name));
        _gameInstanceSynchronizable._scores.Add(user.Index, 0);
        if (multiplayer.GetUsers().Count == multiplayer.MaxPlayers)
        {
            TryStartGame();
        }
    }

    void HandleUserLeft(Multiplayer multiplayer, User user)
    {
        multiplayer.GetUsers().ForEach(u => Debug.Log(u.Name));
        _gameInstanceSynchronizable._scores.Remove(user.Index);
        if (multiplayer.GetUsers().Count != multiplayer.MaxPlayers)
        {
            AbortGameStart();
        }
    }

    private void RefreshAvailableRooms(object state)
    {
        Multiplayer.RefreshRoomList();
    }

    private void TryStartGame()
    {
        State = GameState.GAME_WAITING_TO_START;
        GameStateChanged.Invoke(State);
        _timer = new Timer(StartGame, null, (int)TimeUntilGameStart, 0);
    }

    private void AbortGameStart()
    {
        State = GameState.GAME_WAITING_FOR_PLAYERS;
        GameStateChanged.Invoke(State);
        _timer?.Dispose();
    }

    private void StartGame(object state)
    {
        State = GameState.GAME_RUNNING;
        GameStateChanged.Invoke(State);
        _timer?.Dispose();
    }
}
