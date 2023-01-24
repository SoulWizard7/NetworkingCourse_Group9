using Alteruna;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

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
        if(object.ReferenceEquals(x, y)) return true;

        if(x is null || y is null) return false;

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
    public float TimeUntilStart;

    public GameStateInfo()
    {
        ScoreboardInfo= new List<ScoreboardData>();
        State = GameState.GAME_WAITING_FOR_PLAYERS;
        TimeUntilStart = 5.0f;
    }
}

public static class LoggingExtensions
{
    public static string ToFriendlyString(this GameState gs)
    {
        switch (gs)
        {
            case GameState.GAME_PAUSED:
                return "Paused";
            case GameState.GAME_RUNNING:
                return "Running";
            case GameState.GAME_STOPPED:
                return "Stopped";
            case GameState.GAME_WAITING_FOR_PLAYERS:
                return "Waiting for players";
            case GameState.GAME_WAITING_TO_START:
                return "Waiting to start";
        }
        throw new Exception("Out of bounds");
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

    internal UnityEvent<GameStateInfo> GameStateChanged = new UnityEvent<GameStateInfo>();
    internal UnityEvent<Dictionary<ushort, int>> ScoresUpdated = new UnityEvent<Dictionary<ushort, int>>();
    internal Multiplayer Multiplayer;
    private bool ShouldRefreshRooms = true;

    private void Awake()
    {
        _gameStateSynchronizable = GetComponent<GameInstanceSynchronizable>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Multiplayer.MaxPlayers = 1;
        if(Input.GetKeyDown(KeyCode.Alpha2))
            Multiplayer.MaxPlayers = 2;
        if(Input.GetKeyDown(KeyCode.Alpha3))
            Multiplayer.MaxPlayers = 3;
        if(Input.GetKeyDown(KeyCode.Alpha4))
            Multiplayer.MaxPlayers = 4;

        //Debug.LogWarning(Multiplayer?.MaxPlayers);
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

        ShouldRefreshRooms = true;
        _setupFinished.ForEach(e => e());

        StartCoroutine(nameof(RefreshRooms));
    }

    void HandleRoomJoined(Multiplayer multiplayer, Room room, User user)
    {
        if (multiplayer.GetUsers().Count == 1)
        {
            TryStartGame();
        }
        ShouldRefreshRooms = false;
        Debug.Log(multiplayer.MaxPlayers);
        Multiplayer.MaxPlayers = room.MaxUsers;
        Debug.LogWarning($"Joined room: {room.Name} with playercount of: {room.Users.Count} / {room.MaxUsers}");
        room.Users.ForEach(user => GameStateInfo.ScoreboardInfo.Add(new ScoreboardData { Id = user.Index, Name = user.Name, Score = 0 }));
        GameStateChanged.Invoke(GameStateInfo);
    }

    void HandleRoomLeft(Multiplayer multiplayer) 
    {
        GameStateInfo.ScoreboardInfo.Clear();
        GameStateInfo.State = GameState.GAME_WAITING_FOR_PLAYERS;
        GameStateInfo.TimeUntilStart = 5.0f;
        GameStateChanged.Invoke(GameStateInfo);
        ShouldRefreshRooms = true;
        StartCoroutine(nameof(RefreshRooms));
    }

    public void AddPlayerScore(ushort playerId, int scoreToAdd)
    {
        GameStateInfo.ScoreboardInfo.Find(sb => sb.Id == playerId).Score += scoreToAdd;
    }

    public void SetGameState(GameState gameState)
    {
        GameStateInfo.State = gameState;
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

    private void RefreshAvailableRooms()
    {
        Multiplayer.RefreshRoomList();
    }

    private void TryStartGame()
    {
        GameStateInfo.State = GameState.GAME_WAITING_TO_START;
        StartCoroutine(nameof(GameCountDown));
        GameStateChanged.Invoke(GameStateInfo);
        ShouldRefreshRooms = false;
    }

    IEnumerator RefreshRooms()
    {
        while(ShouldRefreshRooms)
        {
            yield return new WaitForSecondsRealtime(2.0f);
            Debug.Log("Refreshing Rooms");
            RefreshAvailableRooms();
        }
    }

    IEnumerator GameCountDown()
    {
        while (GameStateInfo.TimeUntilStart > 0.0f)
        {
            yield return new WaitForSecondsRealtime(1.0f);
            GameStateInfo.TimeUntilStart -= 1.0f;
        }
        StartGame();
    }

    private void AbortGameStart()
    {
        GameStateInfo.State = GameState.GAME_WAITING_FOR_PLAYERS;
        GameStateChanged.Invoke(GameStateInfo);
        StopCoroutine(nameof(GameCountDown));
        ShouldRefreshRooms = false;
    }

    private void StartGame()
    {
        GameStateInfo.State = GameState.GAME_RUNNING;
        GameStateChanged.Invoke(GameStateInfo);
        ShouldRefreshRooms = false;
    }
}
