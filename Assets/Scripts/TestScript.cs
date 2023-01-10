using Alteruna;
using Alteruna.Trinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private Multiplayer multiplayer;
    [SerializeField] private GameObject projectilePrefab;

    void Start()
    {
        multiplayer = GameObject.Find("Multiplayer").GetComponent<Multiplayer>();
        multiplayer?.RegisterRemoteProcedure("SpawnProjectileReplicated", SpawnProjectileReplicated);
    }

    void SpawnProjectileReplicated(ushort fromUser, ProcedureParameters parameters, uint callId, ITransportStreamReader processor)
    {
        float x = parameters.Get("spawnX", 0.0f);
        float y = parameters.Get("spawnY", 0.0f);
        float z = parameters.Get("spawnZ", 0.0f);
        Vector3 spawnPos = new Vector3(x, y, z);
        GameObject.Instantiate(projectilePrefab, spawnPos, transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ProcedureParameters parameters = new ProcedureParameters();
            parameters.Set("spawnX", transform.position.x);
            parameters.Set("spawnY", transform.position.y);
            parameters.Set("spawnZ", transform.position.z);
            multiplayer?.InvokeRemoteProcedure("SpawnProjectileReplicated", UserId.AllInclusive, parameters);
        }
    }
}
