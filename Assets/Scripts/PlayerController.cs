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

    void Start()
    {
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Confined;
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
