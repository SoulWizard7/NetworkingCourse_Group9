using Alteruna;
using Alteruna.Trinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private Multiplayer multiplayer;
    [SerializeField] private GameObject projectilePrefab;
    private Alteruna.Avatar _avatar;
    private Spawner _spawner;

    void Start()
    {
        _avatar = GetComponent<Alteruna.Avatar>();
        multiplayer = GameObject.Find("Multiplayer").GetComponent<Multiplayer>();
        multiplayer?.RegisterRemoteProcedure("SpawnProjectileReplicated", SpawnProjectileReplicated);
        _spawner = GameObject.Find("Multiplayer").GetComponent<Spawner>();
    }

    void SpawnProjectileReplicated(ushort fromUser, ProcedureParameters parameters, uint callId, ITransportStreamReader processor)
    {
        //float x = parameters.Get("spawnX", 0.0f);
        //float y = parameters.Get("spawnY", 0.0f);
        //float z = parameters.Get("spawnZ", 0.0f);
        //Vector3 spawnPos = new Vector3(x, y, z);
        //var projectile = GameObject.Instantiate(projectilePrefab, spawnPos, transform.rotation);
        //var uIDComponent = projectile.GetComponent<UniqueID>();
        //uIDComponent.MakeUID();
        for (int i = 0; i < _spawner.SpawnableObjects.Count; i++)
        {
            _spawner?.Spawn(i, transform.position, transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _avatar.IsMe)
        {
            ProcedureParameters parameters = new ProcedureParameters();
            //parameters.Set("spawnX", transform.position.x);
            //parameters.Set("spawnY", transform.position.y);
            //parameters.Set("spawnZ", transform.position.z);
            multiplayer?.InvokeRemoteProcedure("SpawnProjectileReplicated", UserId.AllInclusive, parameters);

        }
    }
}
