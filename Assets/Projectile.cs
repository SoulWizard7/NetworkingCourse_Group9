using Alteruna;
using Alteruna.Trinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float Speed = 10.0f;

    private Multiplayer _multiplayer;
    private TransformSynchronizable _transformSynchronizable;
    private Spawner _spawner;

    void Start()
    {
        _multiplayer = GameObject.Find("Multiplayer").GetComponent<Multiplayer>();
        _transformSynchronizable = GetComponent<TransformSynchronizable>();
        _spawner = GameObject.Find("Multiplayer").GetComponent<Spawner>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * Speed * Time.deltaTime;
        if(Mathf.Abs(transform.position.x) > 20.0f || Mathf.Abs(transform.position.y) > 20.0f)
        {
            _spawner.Despawn(gameObject);
        }
    }
}
