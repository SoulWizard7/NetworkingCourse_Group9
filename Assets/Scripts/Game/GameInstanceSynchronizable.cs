using Alteruna;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameInstanceSynchronizable : Synchronizable
{
    private GameInstance _gameInstance;

    internal GameStateInfo GameInfo = new GameStateInfo();

    private void Awake()
    {
        _gameInstance = GetComponent<GameInstance>();
    }

    [ContextMenu("Change game state to running")]
    public void UpdateGameState()
    {

    }

    [ContextMenu("Add score to player 1")]
    public void AddScoreToPlayer() 
    {
        _gameInstance.AddPlayerScore(0, 10);
    }

    //Send data
    public override void AssembleData(Writer writer, byte LOD = 100)
    {
        writer.WriteObject(GameInfo);
        Debug.Log("Send new Data to other clients!");
    }

    //Recieve data
    public override void DisassembleData(Reader reader, byte LOD = 100)
    {
        GameInfo = (GameStateInfo)reader.ReadObject();
        Debug.Log("Updated data");
        _gameInstance.GameStateInfo.State = GameInfo.State;
        _gameInstance.GameStateInfo.ScoreboardInfo = GameInfo.ScoreboardInfo.ConvertAll(s => new ScoreboardData { Id = s.Id, Name = s.Name, Score = s.Score });
        _gameInstance.GameStateChanged.Invoke(GameInfo);
    }

    bool CompareInfo(GameStateInfo info1, GameStateInfo info2) 
    {
        //If states and gamestate info matches remote and local, return true and do nothing, otherwise return false and adjust
        return !info1.ScoreboardInfo.Except(info2.ScoreboardInfo, new ScoreboardComparer()).Any() && info1.ScoreboardInfo.Count == info2.ScoreboardInfo.Count && info1.State == info2.State;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(50, 50, 200, 200), GameInfo.ScoreboardInfo.Count().ToString());
    }

    private void Update()
    {
        if (!CompareInfo(GameInfo, _gameInstance.GameStateInfo))
        {
            Debug.Log("Local is newer");
            GameInfo.State = _gameInstance.GameStateInfo.State;
            GameInfo.ScoreboardInfo = _gameInstance.GameStateInfo.ScoreboardInfo.ConvertAll(s => new ScoreboardData { Id = s.Id, Name = s.Name, Score = s.Score });
            _gameInstance.GameStateChanged.Invoke(_gameInstance.GameStateInfo);
            Commit();
        }
        base.SyncUpdate();
    }
}
