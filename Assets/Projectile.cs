using System;
using Alteruna;
using Alteruna.Trinity;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Avatar = Alteruna.Avatar;

public class Projectile : MonoBehaviour
{
    public Multiplayer multiplayer;
    public User user;
    public Spawner spawner;
    
    

    public void Update()
    {
        if (user == multiplayer.Me)
        {
            //_rb.position += (Vector2)_rb.transform.up * (8 * Time.deltaTime);
            transform.position += transform.up * (8 * Time.deltaTime);
            
            //Vector2 projPos = Camera.main.WorldToViewportPoint(_rb.position);
            Vector2 projPos = Camera.main.WorldToViewportPoint(transform.position);
            if(projPos.x > 1.0f || projPos.x < .0f || projPos.y > 1.0f || projPos.y < .0f)
            {
                spawner.Despawn(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        /*
        var avatar = col.gameObject.GetComponent<Avatar>();
        if (avatar)
        {
            if (!avatar.IsMe)
            {
                //gameObject.SetActive(false);
            } 
        }*/

        if (user == multiplayer.Me)
        {
            var asteroid = col.gameObject.GetComponent<Asteroid>();
            if (asteroid)
            {
                asteroid.OnAsteroidDestroy();
            }
        }
    }
}
