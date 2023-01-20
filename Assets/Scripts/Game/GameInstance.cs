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

[Serializable]
public class ScoreboardData
{
    public ushort Id;
    public string Name;
    public int Score;
}

public class ScoreboardComparer : IEqualityComparer<ScoreboardData>
{
    public bool Equals(ScoreboardData x, ScoreboardData y)
    {
        //Debug.Log($"{x.Id} {y.Id}");
        //Debug.Log($"{x.Name} {y.Name}");
        //Debug.Log($"{x.Score} {y.Score}");

        if(object.ReferenceEquals(x, y)) return true;

        if(x is null || y is null) return false;

        Debug.Log($"Comparison results in: {x.Score == y.Score && x.Name == y.Name && x.Id == y.Id}");
        return x.Score == y.Score && x.Name == y.Name && x.Id == y.Id;
    }

    public int GetHashCode(ScoreboardData obj)
    {
        ReferenceEquals(obj, null);

        int hashId = obj.Id.GetHashCode();

        int hashName = obj.Name == null ? 0 : obj.Name.GetHashCode();

        int hashScore = obj.Score.GetHashCode();

        return hashId ^ hashName ^ hashScore;
    }
}

[Serializable]
public class GameStateInfo
{
    public GameState State;
    public List<ScoreboardData> ScoreboardInfo;

    public GameStateInfo()
    {
        ScoreboardInfo= new List<ScoreboardData>();
        State = GameState.GAME_WAITING_FOR_PLAYERS;
    }
}

[RequireComponent(typeof(GameInstanceSynchronizable))]
public class GameInstance : MonoBehaviour
{
    [SerializeField] private Multiplayer MultiplayerPrefab;

    private List<SetupFinished> _setupFinished = new List<SetupFinished>();

    private TextChatSynchronizable _textChat;
    private GameInstanceSynchronizable _gameStateSynchronizable;

    public List<User> Users { get => Multiplayer.GetUsers(); }
    public delegate void SetupFinished();

    public event SetupFinished OnConnected
    {
        add => _setupFinished.Add(value);
        remove => _setupFinished.Remove(value);
    }

    public GameStateInfo GameStateInfo = new GameStateInfo();
    public float TimeUntilGameStart = 5.0f;

    public Timer _timer;

    internal UnityEvent<GameStateInfo> GameStateChanged = new UnityEvent<GameStateInfo>();
    internal UnityEvent<Dictionary<ushort, int>> ScoresUpdated = new UnityEvent<Dictionary<ushort, int>>();
    internal Multiplayer Multiplayer;

    private void Awake()
    {
        _gameStateSynchronizable = GetComponent<GameInstanceSynchronizable>();
    }

    internal void Setup(string name, int playerCount)
    {
        Multiplayer = Instantiate(MultiplayerPrefab, Vector3.zero, Quaternion.identity);
        Multiplayer.SetUsername(name);
        Multiplayer.MaxPlayers = (ushort)playerCount;
        Multiplayer.Connected.AddListener(SetupMultiplayerListeners);
        Multiplayer.Disconnected.AddListener((mp, endpoint) => Debug.Log("DISCONNECTED"));
        _gameStateSynchronizable.enabled = true;
    }

    void SetupMultiplayerListeners(Multiplayer multiplayer, Endpoint endpoint)
    {

        Multiplayer.OtherUserJoined.AddListener(HandleUserJoined);

        Multiplayer.RoomJoined.AddListener(HandleRoomJoined);

        Multiplayer.RoomLeft.AddListener(HandleRoomLeft);

        Multiplayer.OtherUserLeft.AddListener(HandleUserLeft);

        _timer = new Timer(RefreshAvailableRooms, null, 0, 2000);
        _setupFinished.ForEach(e => e());
    }

    void HandleRoomJoined(Multiplayer multiplayer, Room room, User user)
    {
        _timer?.Dispose();
        room.Users.ForEach(user => GameStateInfo.ScoreboardInfo.Add(new ScoreboardData { Id = user.Index, Name = user.Name, Score = 0 }));
        GameStateChanged.Invoke(GameStateInfo);
    }

    void HandleRoomLeft(Multiplayer multiplayer) 
    {
        GameStateInfo.ScoreboardInfo.Clear();
        GameStateChanged.Invoke(GameStateInfo);
    }

    void HandleUserJoined(Multiplayer multiplayer, User user)
    {
        if (multiplayer.GetUsers().Count == multiplayer.MaxPlayers)
        {
            TryStartGame();
        }
    }

    void HandleUserLeft(Multiplayer multiplayer, User user)
    {
        GameStateInfo.ScoreboardInfo.Remove(GameStateInfo.ScoreboardInfo.Find(i => i.Id == user.Index));
        GameStateChanged.Invoke(GameStateInfo);
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
        GameStateInfo.State = GameState.GAME_WAITING_TO_START;
        GameStateChanged.Invoke(GameStateInfo);
        _timer = new Timer(StartGame, null, (int)TimeUntilGameStart, 0);
    }

    private void AbortGameStart()
    {
        GameStateInfo.State = GameState.GAME_WAITING_FOR_PLAYERS;
        GameStateChanged.Invoke(GameStateInfo);
        _timer?.Dispose();
    }

    private void StartGame(object state)
    {
        GameStateInfo.State = GameState.GAME_RUNNING;
        GameStateChanged.Invoke(GameStateInfo);
        _timer?.Dispose();
    }
}
