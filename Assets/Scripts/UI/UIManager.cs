using Alteruna;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MenuType
{
    MENU_IngameHUD,
    MENU_RoomSelect,
    MENU_MainMenu,
    MENU_PauseMenu,
    MENU_Scoreboard
}

public class UIManager : MonoBehaviour
{
    public int PlayerCount = 4;

    [SerializeField] private List<TMP_Text> _teamHUD;
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private Slider _playerCountSlider;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _hud;
    [SerializeField] private GameObject _roomPanel;
    [SerializeField] private GameObject _scoreBoard;
    [SerializeField] private ScoreboardPlayer _sbPlayerPrefab;
    [SerializeField] private Button _roomButtonPrefab;
    [SerializeField] private Button _startButton;
    [SerializeField] private TMP_Text _gameStateText;
    [SerializeField] private TMP_Text _playerCountText;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _leaveButton;
    
    private List<ScoreboardPlayer> _scoreBoardPlayers = new List<ScoreboardPlayer>();
    private GameInstance _gameInstance;

    private List<Button> _roomButtons = new List<Button>();

    private void Awake()
    {
        _gameInstance = GameObject.Find("GameInstance").GetComponent<GameInstance>();
    }

    void Start()
    {
        SetupButtonListeners();
    }

    void SetupButtonListeners()
    {
        _startButton.onClick.AddListener(() =>
        {
            _gameInstance.Multiplayer.JoinOnDemandRoom();
            
            ShowMenu(MenuType.MENU_IngameHUD);
        });

        _playerCountSlider.value = PlayerCount;
        _playerCountText.text = $"Amount of players: {_playerCountSlider.value}";
        _playerCountSlider.onValueChanged.AddListener(e => {
            PlayerCount = (int)e;
            _playerCountText.text = $"Amount of players: {PlayerCount}";
        });

        _nameInput.onSubmit.AddListener(name =>
        {
            ShowMenu(MenuType.MENU_RoomSelect);
            _gameInstance.Setup(name, PlayerCount);
            _gameInstance.OnConnected += SetupListeners;
        });

        _resumeButton.onClick.AddListener(() => ShowMenu(MenuType.MENU_IngameHUD));
        _leaveButton.onClick.AddListener(() =>
        {
            _gameInstance.Multiplayer.CurrentRoom.Leave();
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
            btnText.text = $"{room.Name} {room.Users.Count} / {room.MaxUsers}";
            btn.transform.position += new Vector3(0.0f, room.ID * -30, 0.0f);
            _roomButtons.Add(btn);
            btn.onClick.AddListener(() => 
            { 
                room.Join();
                ShowMenu(MenuType.MENU_IngameHUD);
            });
        }
    }

    void SetupListeners()
    {
        _gameInstance.Multiplayer.RoomListUpdated.AddListener(ShowAvailableRooms);
        _gameInstance.Multiplayer.RoomLeft.AddListener(mp => ShowMenu(MenuType.MENU_RoomSelect));
        _gameInstance.GameStateChanged.AddListener(RedrawHUD);
    }

    public void ShowMenu(MenuType menuType)
    {
        switch (menuType)
        {
            case MenuType.MENU_IngameHUD:
                _roomPanel.SetActive(false);
                _mainMenu.SetActive(false);
                _hud.SetActive(true);
                _scoreBoard.SetActive(false);
                _pauseMenu.SetActive(false);
                break;
            case MenuType.MENU_MainMenu:
                _roomPanel.SetActive(false);
                _mainMenu.SetActive(true);
                _hud.SetActive(false);
                _scoreBoard.SetActive(false);
                _pauseMenu.SetActive(false);
                break;
            case MenuType.MENU_RoomSelect:
                _roomPanel.SetActive(true);
                _mainMenu.SetActive(false);
                _hud.SetActive(false);
                _scoreBoard.SetActive(false);
                _pauseMenu.SetActive(false);
                break;
            case MenuType.MENU_Scoreboard:
                _roomPanel.SetActive(false);
                _mainMenu.SetActive(false);
                _hud.SetActive(false);
                _scoreBoard.SetActive(true);
                _pauseMenu.SetActive(false);
                break;
            case MenuType.MENU_PauseMenu:
                _pauseMenu.SetActive(true);
                _roomPanel.SetActive(false);
                _mainMenu.SetActive(false);
                _hud.SetActive(false);
                _scoreBoard.SetActive(false);
                break;
        }
    }

    void RedrawHUD(GameStateInfo gameStateInfo)
    {
        _scoreBoardPlayers.ForEach(e => Destroy(e.gameObject));
        _scoreBoardPlayers.Clear();
        gameStateInfo.ScoreboardInfo.ForEach(e =>
        {
            var sbPlayer = Instantiate(_sbPlayerPrefab, _scoreBoard.transform);
            sbPlayer.transform.position += new Vector3(0.0f, e.Id * -30, 0.0f);
            sbPlayer.SetPlayerInfo(e.Name, e.Score);
            _scoreBoardPlayers.Add(sbPlayer);
        });

        if (_gameInstance.Multiplayer.CurrentRoom == null) return;

        if(_gameInstance.GameStateInfo.State != GameState.GAME_WAITING_TO_START)
        {
            _gameStateText.text = $"{gameStateInfo.State.ToFriendlyString()} \n {_gameInstance.Multiplayer.CurrentRoom.Users.Count} / {_gameInstance.Multiplayer.CurrentRoom.MaxUsers} joined";
        }
        else
        {
            _gameStateText.text = $"Game starting in {_gameInstance.GameStateInfo.TimeUntilStart}";
        }
    }
}
