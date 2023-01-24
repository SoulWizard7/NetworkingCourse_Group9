using System;
using Alteruna;
using Alteruna.Trinity;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Avatar = Alteruna.Avatar;

public class Shooting : MonoBehaviour
{
    private Multiplayer _multiplayer;
    private Spawner _spawner;
    private Alteruna.Avatar _avatar;
    
    private GameInstance _gameInstance;

    private List<GameObject> projectiles;

    void Start()
    {
        _gameInstance = GameObject.Find("GameInstance").GetComponent<GameInstance>();
        _multiplayer = _gameInstance.Multiplayer;
        _spawner = _multiplayer.GetComponent<Spawner>();
        _avatar = GetComponent<Alteruna.Avatar>();
        _multiplayer.RegisterRemoteProcedure("HitFunction", HitFuctionRPC);
        projectiles = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_avatar.IsMe) return;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnProjectile();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            SpawnAsteroid();
        }
    }
    
    public void SpawnAsteroid()
    {
        _spawner.Spawn(2, new Vector3(0, 2, 0));
    }

    void SpawnProjectile()
    {
        GameObject proj = _spawner.Spawn(0, transform.position + (transform.up * 0.5f), transform.rotation);
        projectiles.Add(proj);
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        Projectile proj = col.gameObject.GetComponent<Projectile>();

        if (proj)
        {
            /*
            if (_avatar.IsMe)
            {
                if (proj.user != _multiplayer.Me)
                {
                    
                    ProcedureParameters parameters = new ProcedureParameters();
                    parameters.Set("hitPlayer", _multiplayer.Me.Index);
                    parameters.Set("projectileOwner", proj.user.Index);
                    parameters.Set("projectileID", proj.GetComponent<UniqueID>().UIDString);
            
                    //Debug.Log("player " + _multiplayer.Me.Index + "  " + proj.GetComponent<UniqueID>().UIDString);
            
                    _multiplayer.InvokeRemoteProcedure("HitFunction", UserId.AllInclusive, parameters);
            
                    //_spawner.Despawn(proj.gameObject);
                }
            }
            */
            if (proj.user == _multiplayer.Me)
            {
                if(GetComponent<Avatar>().Possessor.Index == proj.user.Index) return;
                
                ProcedureParameters parameters = new ProcedureParameters();
                parameters.Set("hitPlayer", GetComponent<Avatar>().Possessor.Index);
                parameters.Set("projectileOwner", proj.user.Index);

                _multiplayer.InvokeRemoteProcedure("HitFunction", UserId.AllInclusive, parameters);
                _spawner.Despawn(proj.gameObject);
            }
        }
    }
    
    void HitFuctionRPC(ushort fromUser, ProcedureParameters parameters, uint callId, ITransportStreamReader processor)
    {
        ushort hitPlayer = parameters.Get("hitPlayer", (ushort)666);
        ushort projectileOwner = parameters.Get("projectileOwner", (ushort)666);

        Debug.Log("player " + projectileOwner + " hit player " + hitPlayer);

        if (projectileOwner == _multiplayer.Me)
        {
            _gameInstance.AddPlayerScore(projectileOwner, 1);
        }
        
        if (hitPlayer == _multiplayer.Me)
        {
            Debug.Log("I am hit");
        }
    }
}