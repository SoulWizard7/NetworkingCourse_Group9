using Alteruna;
using Alteruna.Trinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Avatar = Alteruna.Avatar;

public class Shooting : MonoBehaviour
{
    private Multiplayer multiplayer;
    private Alteruna.Avatar _avatar;
    private Spawner _spawner;
    

    public int score = 0;

    void Start()
    {
        _avatar = GetComponent<Alteruna.Avatar>();
        multiplayer = GameObject.Find("Multiplayer").GetComponent<Multiplayer>();
        _spawner = multiplayer.GetComponent<Spawner>();
        
        multiplayer.RegisterRemoteProcedure("HitFunction", HitFuctionRPC);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_avatar.IsMe) return;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnProjectile();
        }
    }

    void SpawnProjectile()
    {
        _spawner.Spawn(0, transform.position + (transform.up * 0.25f), transform.rotation);
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!_avatar.IsMe) return;
        Projectile proj = col.gameObject.GetComponent<Projectile>();

        if (proj && proj.user != multiplayer.Me)
        {
            ProcedureParameters parameters = new ProcedureParameters();
            parameters.Set("hitPlayer", multiplayer.Me.Index);
            parameters.Set("projectileOwner", proj.user.Index);
            string id = proj.ids;
            parameters.Set("projectileID", id);
            multiplayer.InvokeRemoteProcedure("HitFunction", UserId.AllInclusive, parameters);
            _spawner.Despawn(col.gameObject);
        }
    }
    
    void HitFuctionRPC(ushort fromUser, ProcedureParameters parameters, uint callId, ITransportStreamReader processor)
    {
        ushort hitPlayer = parameters.Get("hitPlayer", (ushort)666);
        ushort projectileOwner = parameters.Get("projectileOwner", (ushort)666);
        string id = parameters.Get("projectileID", "butt");
        
        Debug.Log("player " + projectileOwner + " hit player " + hitPlayer);

        
        if (hitPlayer == multiplayer.Me)
        {
            Debug.Log("I am hit");
        }
    }
}