using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardPlayer : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI PlayerName;
    [SerializeField] private TMPro.TextMeshProUGUI PlayerScore;

    internal void SetPlayerInfo(string playerName, int playerScore)
    {
        PlayerName.text = playerName;
        PlayerScore.text = playerScore.ToString();
    }

    internal void UpdateScore(int newScore)
    {
        PlayerScore.text = newScore.ToString();
    }
}
