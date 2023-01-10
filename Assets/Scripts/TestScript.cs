using Alteruna;
using Alteruna.Trinity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private Multiplayer multiplayer;

    void Start()
    {
        multiplayer = GameObject.Find("Multiplayer").GetComponent<Multiplayer>();
        multiplayer?.RegisterRemoteProcedure("MyReplicatedFunction", MyReplicatedFunction);
    }

    void MyReplicatedFunction(ushort fromUser, ProcedureParameters parameters, uint callId, ITransportStreamReader processor)
    {
        string myValue = parameters.Get("greetings", "");
        int leetValue = parameters.Get("Leet value", 0);
        Debug.Log($"printing {leetValue} from {fromUser}");
    }

    // Update is called once per frame
    void Update()
    {
        ProcedureParameters parameters = new ProcedureParameters();
        parameters.Set("greetings", "greetings from noob player");
        parameters.Set("Leet value", 1337);
        multiplayer?.InvokeRemoteProcedure("MyReplicatedFunction", UserId.All, parameters);
    }
}
