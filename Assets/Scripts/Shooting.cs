using Alteruna;
using Alteruna.Trinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Avatar = Alteruna.Avatar;

public class Shooting : MonoBehaviour
{
    private Multiplayer _multiplayer;
    private Spawner _spawner;
    private Alteruna.Avatar _avatar;
    

    public int score = 0;

    void Start()
    {
        _avatar = GetComponent<Alteruna.Avatar>();
        _multiplayer = GameObject.Find("Multiplayer").GetComponent<Multiplayer>();
        _spawner = _multiplayer.GetComponent<Spawner>();
        
        _multiplayer.RegisterRemoteProcedure("HitFunction", HitFuctionRPC);
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
        _spawner.Spawn(0, transform.position + (transform.up * 0.25f), transform.rotation);
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!_avatar.IsMe) return;
        Projectile proj = col.gameObject.GetComponent<Projectile>();

        if (proj && proj.user != _multiplayer.Me)
        {
            ProcedureParameters parameters = new ProcedureParameters();
            parameters.Set("hitPlayer", _multiplayer.Me.Index);
            parameters.Set("projectileOwner", proj.user.Index);
            string id = proj.ids;
            parameters.Set("projectileID", id);
            _multiplayer.InvokeRemoteProcedure("HitFunction", UserId.AllInclusive, parameters);
            _spawner.Despawn(col.gameObject);
        }
    }
    
    void HitFuctionRPC(ushort fromUser, ProcedureParameters parameters, uint callId, ITransportStreamReader processor)
    {
        ushort hitPlayer = parameters.Get("hitPlayer", (ushort)666);
        ushort projectileOwner = parameters.Get("projectileOwner", (ushort)666);
        string id = parameters.Get("projectileID", "butt");
        
        Debug.Log("player " + projectileOwner + " hit player " + hitPlayer);

        if (projectileOwner == _multiplayer.Me)
        {
            //UI Update score
        }
        
        if (hitPlayer == _multiplayer.Me)
        {
            Debug.Log("I am hit");
        }
    }
}