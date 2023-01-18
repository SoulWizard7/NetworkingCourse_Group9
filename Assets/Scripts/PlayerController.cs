using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;
using Avatar = Alteruna.Avatar;

public class PlayerController : MonoBehaviour
{
    public float Speed = 5.0f;

    private Alteruna.Avatar _avatar;
    private SpriteRenderer _renderer;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Confined;
        _avatar = GetComponent<Alteruna.Avatar>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (_avatar.IsMe)
        {
            _renderer.color = Color.green;
                
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

            //Mouselook
            Vector2 mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            transform.up = mousePos - new Vector2(transform.position.x, transform.position.y);
        }
    }
}
