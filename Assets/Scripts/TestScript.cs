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
        multiplayer?.RegisterRemoteProcedure("MyReplicatedFunction", MyReplicatedFunction);
        multiplayer?.RegisterRemoteProcedure("SpawnProjectileReplicated", SpawnProjectileReplicated);
    }

    void MyReplicatedFunction(ushort fromUser, ProcedureParameters parameters, uint callId, ITransportStreamReader processor)
    {
        string myValue = parameters.Get("greetings", "");
        int leetValue = parameters.Get("Leet value", 0);
    }

    void SpawnProjectileReplicated(ushort fromUser, ProcedureParameters parameters, uint callId, ITransportStreamReader processor)
    {
        GameObject.Instantiate(projectilePrefab, transform.position, transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        ProcedureParameters parameters = new ProcedureParameters();
        parameters.Set("greetings", "greetings from noob player");
        parameters.Set("Leet value", 1337);
        multiplayer?.InvokeRemoteProcedure("MyReplicatedFunction", UserId.AllInclusive, parameters);
        if(Input.GetKeyDown(KeyCode.Space))
        {
            multiplayer?.InvokeRemoteProcedure("SpawnProjectileReplicated", UserId.AllInclusive, parameters);
        }
    }
}
