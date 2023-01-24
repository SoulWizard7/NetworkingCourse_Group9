using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;
using Alteruna.Trinity;
using Avatar = Alteruna.Avatar;

public class PlayerController : MonoBehaviour
{
    public float Speed = 5.0f;

    private Alteruna.Avatar _avatar;
    private SpriteRenderer _renderer;

    private Camera cam;
    
    private Multiplayer _multiplayer;
    private Spawner _spawner;

    private UIManager _uiManager;
    private GameInstance _gameInstance;

    private Rigidbody2DSynchronizable _rb;

    void Start()
    {
        _gameInstance = GameObject.Find("GameInstance").GetComponent<GameInstance>();
        _uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        cam = Camera.main;

        _rb = GetComponent<Rigidbody2DSynchronizable>();
 
        Cursor.lockState = CursorLockMode.Confined;

        _avatar = GetComponent<Alteruna.Avatar>();
        _renderer = GetComponent<SpriteRenderer>();

        _multiplayer = _gameInstance.Multiplayer;
        _spawner = _multiplayer.GetComponent<Spawner>();
    }

    void Update()
    {
        if (_avatar.IsMe)
        {
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
            float _moveVertical = Input.GetAxis("Vertical") * Speed;
            float _moveHorizontal = Input.GetAxis("Horizontal") * Speed;

            _moveVertical *= Time.deltaTime;
            _moveHorizontal *= Time.deltaTime;
            
            //Constrain to screen
            Vector2 playerPos = cam.WorldToViewportPoint(transform.position);
            if (playerPos.x < 0) _moveHorizontal = Mathf.Clamp(_moveHorizontal, 0, 1);
            else if (playerPos.x > 1) _moveHorizontal = Mathf.Clamp(_moveHorizontal, -1, 0);

            if (playerPos.y < 0) _moveVertical = Mathf.Clamp(_moveVertical, 0, 1);
            else if (playerPos.y > 1) _moveVertical = Mathf.Clamp(_moveVertical, -1, 0);
                
            //Move player
            transform.Translate(_moveHorizontal, _moveVertical, 0, Space.World);
            //_rb.position += new Vector2(_moveHorizontal, _moveVertical);

            //Mouselook
            Vector2 mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            transform.up = mousePos - new Vector2(transform.position.x, transform.position.y);
        }
    }


    private void OnCollisionEnter2D(Collision2D col)
    {
        AsteroidPowerup ap = col.gameObject.GetComponent<AsteroidPowerup>();
        if (ap)
        {
            Debug.Log("powerup");
            StartCoroutine(SuperSpeed());
            _spawner.Despawn(col.gameObject);
        }
    }

    IEnumerator SuperSpeed()
    {
        float originSpeed = Speed;
        Speed *= 1.5f;
        yield return new WaitForSeconds(10);
        Speed = originSpeed;
    }
}
