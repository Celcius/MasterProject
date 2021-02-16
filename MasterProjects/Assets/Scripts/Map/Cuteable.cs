﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class Cuteable : GridEntity
{
    [SerializeField]
    private Transform cutInstance;

    [SerializeField]
    private RoomTileController controller;

    public void Cut()
    {
        if(cutInstance != null)
        {
           Instantiate(cutInstance, transform.position, Quaternion.identity); 
        }
        gameObject.SetActive(false);
        controller.AddFloorPos((Vector2Int)CameraMover.GridPosForWorldPos(transform.position));
    }

    private void OnEnable() 
    {
        controller.RemoveFloorPos((Vector2Int)CameraMover.GridPosForWorldPos(transform.position));
    }
}
