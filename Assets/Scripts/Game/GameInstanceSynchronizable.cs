using Alteruna;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameInstanceSynchronizable : Synchronizable
{
    public Dictionary<ushort, int> _scores = new Dictionary<ushort, int>();
    private Dictionary<ushort, int> _oldscores = new Dictionary<ushort, int>();

    public GameState State = GameState.GAME_WAITING_FOR_PLAYERS;
    private GameState _oldState;

    internal UnityEvent<GameState> GameStateChanged = new UnityEvent<GameState>();
    internal UnityEvent<Dictionary<ushort, int>> ScoreChanged = new UnityEvent<Dictionary<ushort, int>>();

    [ContextMenu("Update player 1 Score")]
    public void UpdatePlayerOneScore()
    {
        _scores[0] += 10;
    }

    [ContextMenu("Update player 2 Score")]
    public void UpdatePlayerTwoScore()
    {
        _scores[1] += 10;
    }

    [ContextMenu("Change game state to running")]
    public void UpdateGameState()
    {
        State = GameState.GAME_RUNNING;
    }

    public override void AssembleData(Writer writer, byte LOD = 100)
    {
        writer.WriteObject(State);
        writer.WriteObject(_scores);
        Debug.Log("Assemble");
    }

    public override void DisassembleData(Reader reader, byte LOD = 100)
    {
        State = (GameState)reader.ReadObject();
        _scores = (Dictionary<ushort, int>)reader.ReadObject();

        if(State != _oldState)
        {
            _oldState = State;
            Debug.Log("State update local!");
            GameStateChanged.Invoke(State);
        }

        Debug.LogWarning(_scores.Count);

        if (_scores.Except(_oldscores).Any())
        {
            _oldscores.Clear();
            _oldscores.AddRange(_scores);
            ScoreChanged.Invoke(_scores);
            Debug.Log("Receive scores!");
        }
    }

    private void Update()
    {
        if(State != _oldState)
        {
            _oldState = State;
            GameStateChanged.Invoke(State);
            Debug.Log("State update remote!");
            Commit();
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            _scores[0] += 15;
        }

        if(_scores.Except(_oldscores).Any())
        {
            _oldscores.Clear();
            _oldscores.AddRange(_scores);
            ScoreChanged.Invoke(_scores);
            Debug.Log("Submit scores!");
            Commit();
        }
        
        base.SyncUpdate();
    }
}
