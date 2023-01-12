using Alteruna;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> _playerHUD;
    [SerializeField] private List<TMP_Text> _teamHUD;
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _hud;

    private Multiplayer _multiplayer;
    private Canvas _selfCanvas;

    void Start()
    {
        _selfCanvas = GetComponent<Canvas>();
        _multiplayer = GameObject.Find("Multiplayer").GetComponent<Multiplayer>();

        _nameInput.onSubmit.AddListener(name =>
        {
            _multiplayer.SetUsername(name);
            _nameInput.gameObject.SetActive(false);
        });

        _multiplayer.RoomJoined.AddListener((multiplayer, room, user) =>
        {
            Debug.Log($"{user.Name} has joined the room with roomname: {room.Name}!");
            room.Users.ForEach(e => _playerHUD[room.Users.Count - 1].text = e.Name);
            _selfCanvas.gameObject.SetActive(true);
        });

        _multiplayer.RoomLeft.AddListener(multiplayer =>
        {
            _playerHUD.ForEach(e =>
            {
                e.text = "Player:";
            });

            _selfCanvas.gameObject.SetActive(false);
        });

        _multiplayer.OtherUserJoined.AddListener((multiplayer, user) =>
        {
            Debug.Log($"{user.Name} has joined the room!");
            _playerHUD[multiplayer.GetUsers().Count - 1].text = user.Name;
        });

        _multiplayer.OtherUserLeft.AddListener((multiplayer, user) =>
        {
            Debug.Log($"{user.Name} has left the room!");
            _multiplayer.CurrentRoom.Users.ForEach(e => _playerHUD[_multiplayer.CurrentRoom.Users.Count - 1].text = user.Name);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
