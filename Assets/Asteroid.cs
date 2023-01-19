using System.Collections;
using System.Collections.Generic;
using Alteruna;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float Speed = 1.0f;
    [SerializeField] private float asteroidYSin = 1.0f;
    
    private Multiplayer _multiplayer;
    private TransformSynchronizable _transformSynchronizable;
    private Spawner _spawner;

    Vector3 direction;

    public User user;
    
    void Start()
    {
        _multiplayer = GameObject.Find("Multiplayer").GetComponent<Multiplayer>();
        _transformSynchronizable = GetComponent<TransformSynchronizable>();
        _spawner = GameObject.Find("Multiplayer").GetComponent<Spawner>();
        
        direction  =-transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.instance.GameStarted)
            return;

        if (user == _multiplayer.Me)
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
        _spawner?.Spawn(1, transform.position, transform.rotation);
        _spawner.Despawn(gameObject);
    }
}

