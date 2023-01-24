using System;
using System.Collections;
using System.Collections.Generic;
using Alteruna;
using UnityEngine;

public class AsteroidPowerup : MonoBehaviour
{
    [SerializeField] private float powerupSpeed;
    public User user;
    public Multiplayer multiplayer;
    public Spawner spawner;
    
    private Rigidbody2DSynchronizable _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2DSynchronizable>();
        spawner = multiplayer.GetComponent<Spawner>();
    }

    void Update()
    {
        if (!_rb) return;
            
        if (user == multiplayer.Me)
        {
            _rb.position += new Vector2(0, -powerupSpeed * Time.deltaTime);
            
            if(_rb.position.y < -10f)
                spawner.Despawn(gameObject);
        }
    }
}
