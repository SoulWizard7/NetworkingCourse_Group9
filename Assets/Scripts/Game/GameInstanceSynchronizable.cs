using Alteruna;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameInstanceSynchronizable : Synchronizable
{
    public Dictionary<ushort, int> _scores = new Dictionary<ushort, int>();
    private Dictionary<ushort, int> _oldscores = new Dictionary<ushort, int>();

    private UIManager _uiManager;
    private Multiplayer _multiplayer;

    private void Awake()
    {
        _uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        _uiManager.onNameEntered += _uiManager_onNameEntered;
    }

    private void _uiManager_onNameEntered(Multiplayer multiplayer)
    {
        _multiplayer = multiplayer;
        _multiplayer.RoomJoined.AddListener((mp, room, user) =>
        {
            _scores.Add(user.Index, 0);
        });

        multiplayer.OtherUserJoined.AddListener((mp, user) =>
        {
            _scores.Add(user.Index, 0);
        });
    }

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

    public override void AssembleData(Writer writer, byte LOD = 100)
    {
        writer.WriteObject(_scores);
    }

    public override void DisassembleData(Reader reader, byte LOD = 100)
    {
        _scores = (Dictionary<ushort, int>)reader.ReadObject();
        _oldscores = _scores;
    }

    private void Update()
    {
        //foreach (var score in _scores)
        //{
        //    Debug.LogWarning($"Key: {score.Key}, Value: {score.Value}");
        //}

        //foreach (var score in _oldscores)
        //{
        //    Debug.LogWarning($"Key: {score.Key}, Value: {score.Value}");
        //}

        if(_scores != _oldscores)
        {
            _oldscores = _scores;
            Debug.LogWarning("SCORES HAS CHANGED, UPDATED ALL UI:S!");
            Commit();
        }
        
        base.SyncUpdate();
    }
}
