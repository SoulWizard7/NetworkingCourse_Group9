using System.Collections;
using System.Collections.Generic;
using Alteruna;
using UnityEngine;
using Alteruna.Trinity;
using Unity.VisualScripting;
using UnityEditor;

public class ProjectileHandler : MonoBehaviour
{
    private Multiplayer _multiplayer;
    private Spawner _spawner;
    private int poolSize;

    void Start()
    {
        _spawner = GetComponent<Spawner>();
        _multiplayer = GetComponent<Multiplayer>();
    }

    public void SetProjectileData(User user, GameObject spawnedGameObject)
    {
        Projectile p = spawnedGameObject.GetComponent<Projectile>();
        if (p)
        {
            p.user = user;
            p.multiplayer = _multiplayer;
            //p.ids = spawnedGameObject.GetComponent<UniqueID>().UIDString;
            p.spawner = _spawner;
        }

        Asteroid asteroid = spawnedGameObject.GetComponent<Asteroid>();
        if (asteroid)
        {
            asteroid.user = user;
            asteroid.multiplayer = _multiplayer;
            asteroid.spawner = _spawner;
        }

        AsteroidPowerup ap = spawnedGameObject.GetComponent<AsteroidPowerup>();
        if (ap)
        {
            ap.user = user;
            ap.multiplayer = _multiplayer;
        }
    }
}
