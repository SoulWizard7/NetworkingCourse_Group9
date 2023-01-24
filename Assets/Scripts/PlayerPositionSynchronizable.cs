using System;
using System.Collections;
using System.Collections.Generic;
using Alteruna;
using UnityEngine;
using Avatar = Alteruna.Avatar;

public class PlayerPositionSynchronizable : Synchronizable
{
    private Avatar _avatar;

    private Vector2 pos;
    private float rot;
    private Rigidbody2D _rb;
    
    public float speed = 1.0F;
    private float startTime;
    private float journeyLength;
    
    private void Start()
    {
        _avatar = GetComponent<Alteruna.Avatar>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public override void AssembleData(Writer writer, byte LOD = 100)
    {
        writer.Write(pos);
        writer.Write(rot);
    }
    
    public override void DisassembleData(Reader reader, byte LOD = 100)
    {
        pos = reader.ReadVector2();
        rot = reader.ReadFloat();
    }

    private void Update()
    {
        if (_avatar.IsMe)
        {
            pos = transform.position;
            rot = transform.rotation.eulerAngles.z;
        }
        else
        {
            float distCovered = (Time.time - startTime) * speed;            
            float fractionOfJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(transform.position, (Vector3)pos, fractionOfJourney);

            transform.localRotation = Quaternion.Euler(0, 0, rot);
        }
        Commit();
        base.SyncUpdate();
    }
}
