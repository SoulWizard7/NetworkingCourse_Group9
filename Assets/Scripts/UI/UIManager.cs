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

internal struct ScoreboardData
{
    internal ushort Id;
    internal string Name;
    internal string Score;
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
    
    private List<ScoreboardPlayer> _scoreBoardPlayers = new List<ScoreboardPlayer>();
    private GameInstance _gameInstance;
    private Canvas _selfCanvas;
    private Timer _timer;

    private List<Button> _roomButtons = new List<Button>();

    private void Awake()
    {
        _gameInstance = GameObject.Find("GameInstance").GetComponent<GameInstance>();
    }

    void Start()
    {
        _selfCanvas = GetComponent<Canvas>();

        _playerCountSlider.value = PlayerCount;
        _playerCountText.text = _playerCountSlider.value.ToString();
        _playerCountSlider.onValueChanged.AddListener(e => { 
            PlayerCount = (int)e;
            _playerCountText.text = PlayerCount.ToString();
        });

        _nameInput.onSubmit.AddListener(name =>
        {
            ShowMenu(MenuType.MENU_RoomSelect);
            _gameInstance.Setup(name, PlayerCount);
            _gameInstance.OnConnected += SetupListeners;
        });

    }

    public void UpdateScoreForPlayer(int playerIndex, int newScore)
    {
        _scoreBoardPlayers[playerIndex].UpdateScore(newScore);
        Debug.Log(playerIndex.ToString());
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
            btn.onClick.AddListener(() => 
            { 
                room.Join();
                ShowMenu(MenuType.MENU_IngameHUD);
            });
        }
    }

    void SetupListeners()
    {
        _startButton.onClick.AddListener(() =>
        {
            _gameInstance.Multiplayer.JoinOnDemandRoom();
            ShowMenu(MenuType.MENU_IngameHUD);
            _timer?.Dispose();
        });

        _gameInstance.GameStateChanged.AddListener(newState => _gameStateText.text = newState.ToString());

        _gameInstance.Multiplayer.RoomJoined.AddListener((multiplayer, room, user) =>
        {
            //room.Users.ForEach(e => _playerHUD[room.Users.Count - 1].text = e.Name);
            room.Users.ForEach(u =>
            {
                var newSbPlayer = Instantiate(_sbPlayerPrefab, _scoreBoard.transform);
                newSbPlayer.transform.position += new Vector3(0.0f, 10.0f * (user.Index + 1), 0.0f);
                newSbPlayer.SetPlayerInfo(u.Name, 0);
                _scoreBoardPlayers.Add(newSbPlayer);
            });

            _gameInstance.ScoresUpdated.AddListener(scoresUpdated =>
            {
                foreach (var score in scoresUpdated)
                {
                    UpdateScoreForPlayer(score.Key, score.Value);
                }

                Debug.Log("Updating scores on HUD!");
            });

            _timer?.Dispose();
            _selfCanvas.gameObject.SetActive(true);
        });

        _gameInstance.Multiplayer.RoomLeft.AddListener(multiplayer =>
        {
            _scoreBoardPlayers.ForEach(sb => Destroy(sb.gameObject));
            _scoreBoardPlayers.Clear();
            _selfCanvas.gameObject.SetActive(false);
        });

        _gameInstance.Multiplayer.OtherUserJoined.AddListener((multiplayer, user) =>
        {
            var newSbItem = Instantiate(_sbPlayerPrefab, _scoreBoard.transform);
            newSbItem.transform.position += new Vector3(0.0f, 10.0f * user.Index, 0.0f);
            newSbItem.SetPlayerInfo(user.Name, 0);
            _scoreBoardPlayers.Add(newSbItem);
        });

        _gameInstance.Multiplayer.OtherUserLeft.AddListener((multiplayer, user) =>
        {
            var sbItem = _scoreBoardPlayers.Find(sb => sb.name == user.Name);
            Destroy(sbItem.gameObject);
            _scoreBoardPlayers.Remove(sbItem);
        });

        _gameInstance.Multiplayer.RoomListUpdated.AddListener(ShowAvailableRooms);
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
                break;
            case MenuType.MENU_MainMenu:
                _roomPanel.SetActive(false);
                _mainMenu.SetActive(true);
                _hud.SetActive(false);
                _scoreBoard.SetActive(false);
                break;
            case MenuType.MENU_RoomSelect:
                _roomPanel.SetActive(true);
                _mainMenu.SetActive(false);
                _hud.SetActive(false);
                _scoreBoard.SetActive(false);
                break;
            case MenuType.MENU_Scoreboard:
                _roomPanel.SetActive(false);
                _mainMenu.SetActive(false);
                _hud.SetActive(false);
                _scoreBoard.SetActive(true);
                break;
        }
    }

    private void OnDestroy()
    {
        _timer?.Dispose();
    }
}
