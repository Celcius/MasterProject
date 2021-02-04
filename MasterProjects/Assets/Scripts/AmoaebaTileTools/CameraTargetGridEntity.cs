using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetGridEntity : GridEntity
{
    [SerializeField]
    private GridEntityVar targetVar;


    public override void OnRoomWillEnter() 
    {
        if(targetVar.Value == this)
        {
            return;
        }
        base.OnRoomWillEnter();
    }

    public override void OnRoomLeave() 
    {
        if(targetVar.Value == this)
        {
            return;
        }
        base.OnRoomLeave();
    }
}
