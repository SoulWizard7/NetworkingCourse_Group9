using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidPowerup : MonoBehaviour
{
    [SerializeField] private float powerupSpeed;
    
    void Update()
    {
        transform.position += new Vector3(0, -powerupSpeed * Time.deltaTime, 0);
    }
}
