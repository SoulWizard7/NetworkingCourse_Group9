using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class PlayerController : MonoBehaviour
{
    public float Speed = 10.0f;
    public float RotationSpeed = 180.0f;

    private Alteruna.Avatar _avatar;
    private SpriteRenderer _renderer;

    private Camera cam;

    private UIManager _uiManager;
    private GameInstance _gameInstance;

    void Start()
    {
        _gameInstance = GameObject.Find("GameInstance").GetComponent<GameInstance>();
        _uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        cam = Camera.main;
        //Cursor.lockState = CursorLockMode.Confined;
        // Get components
        _avatar = GetComponent<Alteruna.Avatar>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Only let input affect the avatar if it belongs to me
        if (_avatar.IsMe)
        {
            // Set the avatar representing me to be green
            _renderer.color = Color.green;

            if(Input.GetKeyDown(KeyCode.Tab) && _gameInstance.GameStateInfo.State != GameState.GAME_STOPPED)
            {
                _uiManager.ShowMenu(MenuType.MENU_Scoreboard);
            }

            if(Input.GetKeyUp(KeyCode.Tab) && _gameInstance.GameStateInfo.State != GameState.GAME_STOPPED) 
            {
                _uiManager.ShowMenu(MenuType.MENU_IngameHUD);
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                _uiManager.ShowMenu(MenuType.MENU_PauseMenu);
            }

            //Movement
            float _translation = Input.GetAxis("Vertical") * Speed;
            float _strafe = Input.GetAxis("Horizontal") * Speed;

            _translation *= Time.deltaTime;
            _strafe *= Time.deltaTime;
            transform.Translate(_strafe, _translation, 0, Space.World);

            //Mouselook
            Vector2 mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            transform.up = mousePos - new Vector2(transform.position.x, transform.position.y);
        }
    }
}
