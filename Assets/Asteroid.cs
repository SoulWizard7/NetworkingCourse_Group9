using System.Collections;
using System.Collections.Generic;
using Alteruna;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float Speed = 1.0f;
    [SerializeField] private float asteroidYSin = 1.0f;
    
    public Multiplayer multiplayer;
    public Spawner spawner;

    Vector3 direction;

    public User user;

    private Rigidbody2DSynchronizable _rb;
    
    void Start()
    {
        direction  =-Vector3.right;
        _rb = GetComponent<Rigidbody2DSynchronizable>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(!GameManager.instance.GameStarted) return;

        if (user == multiplayer.Me)
        {
            if (transform.position.x > 10f)
            {
                direction = -Vector3.right;
            }
            else if(transform.position.x < -10f)
            {
                direction = Vector3.right;
            }
        
            _rb.position += (Vector2)direction  * Speed * Time.deltaTime;
            _rb.position += new Vector2(0, transform.position.y * Mathf.Sin(asteroidYSin * Time.time)  * Time.deltaTime);
        }
    }

    public void OnAsteroidDestroy()
    {
        spawner?.Spawn(1, transform.position, transform.rotation);
        GameManager.instance.activeAsteroid = false;
        spawner.Despawn(gameObject);
    }
}

