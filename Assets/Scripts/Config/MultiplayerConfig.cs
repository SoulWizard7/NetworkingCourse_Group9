using Alteruna;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerConfig : MonoBehaviour
{
    [SerializeField] private Multiplayer _multiplayer;
    private UIManager _uiManager;

    private void Awake()
    {
        _uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        _multiplayer.SetUsername(_uiManager.UserName);
    }
}
