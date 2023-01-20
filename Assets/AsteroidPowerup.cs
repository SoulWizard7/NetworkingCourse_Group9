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
    
    private Rigidbody2DSynchronizable _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2DSynchronizable>();
    }

    void Update()
    {
        if (user == multiplayer.Me)
        {
            _rb.position += new Vector2(0, -powerupSpeed * Time.deltaTime);
        }
    }
}
