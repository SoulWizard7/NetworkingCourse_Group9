using Alteruna;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameInstanceSynchronizable : Synchronizable
{
    private GameInstance _gameInstance;
    public List<ScoreboardData> Scores = new List<ScoreboardData>();
    public List<ScoreboardData> OldScores = new List<ScoreboardData>();

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

    }

    //Send data
    public override void AssembleData(Writer writer, byte LOD = 100)
    {
        writer.WriteObject(Scores);
        Debug.Log("Send new Data to other clients!");
    }

    //Recieve data
    public override void DisassembleData(Reader reader, byte LOD = 100)
    {
        Scores = (List<ScoreboardData>)reader.ReadObject();
        Debug.Log("Updated data");
        OldScores = Scores.ConvertAll(s => new ScoreboardData { Id = s.Id, Name = s.Name, Score = s.Score });
    }

    bool CompareInfo(List<ScoreboardData> info1, List<ScoreboardData> info2) 
    {
        //If states and gamestate info matches remote and local, return true and do nothing, otherwise return false and adjust
        if (!info1.Except(info2, new ScoreboardComparer()).Any() && info1.Count == info2.Count)
        {
            Debug.Log("Scoreboards dont differ, dont update pls!");
            return true;
        } else
        {
            return false;
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(50, 50, 200, 200), Scores.Count().ToString());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U)) 
        {
            Scores.Add(new ScoreboardData { Id = 1, Name = "Bruh", Score =  UnityEngine.Random.Range(1, 105)});
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Scores[1].Score += 10;

            Debug.Log(Scores.Except(OldScores, new ScoreboardComparer()).Any());

            Debug.Log($"Score of index 1 is: {Scores[1].Score}");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Scores.RemoveAt(0);
        }
        if (!CompareInfo(Scores, OldScores))
        {
            Debug.Log("Local is newer");
            OldScores = Scores.ConvertAll(s => new ScoreboardData { Id = s.Id, Name = s.Name, Score = s.Score });
            Commit();
        }
        base.SyncUpdate();
    }
}
