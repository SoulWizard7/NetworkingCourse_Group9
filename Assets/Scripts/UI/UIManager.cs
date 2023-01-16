using Alteruna;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> _playerHUD;
    [SerializeField] private List<TMP_Text> _teamHUD;
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _hud;
    [SerializeField] private GameObject _roomPanel;
    [SerializeField] private Multiplayer MultiplayerPrefab;
    [SerializeField] private Button _roomButtonPrefab;
    [SerializeField] private Button _startButton;
    
    private Multiplayer _multiplayer;
    private Canvas _selfCanvas;
    private Timer _timer;

    private List<Button> _roomButtons = new List<Button>();

    void Start()
    {
        _selfCanvas = GetComponent<Canvas>();

        _nameInput.onSubmit.AddListener(name =>
        {
            _multiplayer = Instantiate(MultiplayerPrefab, Vector3.zero, Quaternion.identity);
            _multiplayer.SetUsername(_nameInput.text);
            _nameInput.gameObject.SetActive(false);
            _roomPanel.SetActive(true);
            _multiplayer.Connected.AddListener(SetupMultiplayerListeners);
        });
    }

    void ShowAvailableRooms(Multiplayer multiplayer)
    {
        if(_roomButtons.Count > 0)
        {
            _roomButtons.ForEach(button =>
            {
                Destroy(button.gameObject);
            });
            _roomButtons.Clear();
        }

        foreach (var room in multiplayer.AvailableRooms)
        {
            Button btn = Instantiate(_roomButtonPrefab, _roomPanel.transform);
            TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = room.Name;
            btn.transform.position += Vector3.down * 2.0f;
            _roomButtons.Add(btn);
        }

        Debug.Log($"Rooms loaded! roomcount: {multiplayer.AvailableRooms.Count}");
    }

    void SetupMultiplayerListeners(Multiplayer multiplayer, Endpoint endpoint)
    {
        Debug.Log("CONNECTED!!");

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

        _startButton.onClick.AddListener(() =>
        {
            _multiplayer.JoinOnDemandRoom();
            _roomPanel.SetActive(false);
            _hud.SetActive(true);
            _timer.Dispose();
        });

        _multiplayer.RoomListUpdated.AddListener(ShowAvailableRooms);

        _timer = new Timer(RefreshAvailableRooms, null, 0, 2000);
    }

    private void RefreshAvailableRooms(object state)
    {
        _multiplayer.RefreshRoomList();
    }

    private void OnDestroy()
    {
        _timer.Dispose();
    }
}
