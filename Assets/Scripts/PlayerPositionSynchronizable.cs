using System;
using System.Collections;
using System.Collections.Generic;
using Alteruna;
using UnityEngine;
using Avatar = Alteruna.Avatar;

public class PlayerPositionSynchronizable : Synchronizable
{
    private Avatar _avatar;

    private Vector2 lastPos;
    private Vector2 pos;
    private float rot;
    private Vector2 vel;
    private Rigidbody2D _rb;

    private int framesBetweenUpdate = 60;
    private int curFrame = 0;
    
    private float lerpTime = 1f;
    private float currentLerpTime = 1f;

    private float moveDistance = 0f;
    
    
    private void Start()
    {
        _avatar = GetComponent<Alteruna.Avatar>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public override void AssembleData(Writer writer, byte LOD = 100)
    {
        writer.Write(pos);
        writer.Write(rot);
        writer.Write(vel);
    }
    
    public override void DisassembleData(Reader reader, byte LOD = 100)
    {
        lastPos = transform.position;
        pos = reader.ReadVector2();
        rot = reader.ReadFloat();
        vel = reader.ReadVector2();
        pos += vel * 2;
        currentLerpTime = 0f;
    }

    private void Update()
    {
        if (_avatar.IsMe)
        {
            pos = _rb.position;
            rot = transform.rotation.eulerAngles.z;
            vel = _rb.velocity;

            if (curFrame < framesBetweenUpdate)
            {
                curFrame++;
            }
            else
            {
                curFrame = 0;
                
            }
        }
        else
        {
            moveDistance = Vector3.Distance(lastPos, pos);

            currentLerpTime += Time.deltaTime * 40;
            if (currentLerpTime > lerpTime) 
            {
                currentLerpTime = lerpTime;
            }

            float perc = currentLerpTime / lerpTime;
            transform.position = Vector3.LerpUnclamped(lastPos, pos, perc);

            transform.localRotation = Quaternion.Euler(0, 0, rot);
        }
        
        Commit();
        base.SyncUpdate();
    }
}
