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
    
    void Start()
    {
        direction  =-transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        //if(!GameManager.instance.GameStarted) return;

        if (user == multiplayer.Me)
        {
            if (transform.position.x > 10f)
            {
                direction = -transform.right;
            }
            else if(transform.position.x < -10f)
            {
                direction = transform.right;
            }
        
            transform.position += direction  * Speed * Time.deltaTime;
            transform.position += new Vector3(0, transform.position.y * Mathf.Sin(asteroidYSin * Time.time)  * Time.deltaTime, 0);
        }
        
        //if(Input.GetKeyDown(KeyCode.B)) OnAsteroidDestroy();
    }

    public void OnAsteroidDestroy()
    {
        spawner?.Spawn(1, transform.position, transform.rotation);
        spawner.Despawn(gameObject);
    }
}

