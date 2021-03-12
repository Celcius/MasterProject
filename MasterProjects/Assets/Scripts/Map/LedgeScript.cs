using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;
using UnityEngine.Tilemaps;

public class LedgeScript : PlayerCollideable
{
    [SerializeField]
    private RoomTileController roomController;

    [SerializeField]
    private Vector2Int searchDir = new Vector2Int(0,-1);

    [SerializeField]
    private TileBase[] validDropTiles;

    [SerializeField]
    private TileBase[] acceptedTiles;

    Vector3Int myPos;
    Vector3Int posBelow;
    HashSet<string> acceptedHash = new HashSet<string>();
    HashSet<string> validDropHash = new HashSet<string>();

    private void Start()
    {
        myPos = CameraMover.GridPosForWorldPos(transform.position);
        if(acceptedTiles != null)
        {
            foreach(TileBase tile in acceptedTiles)
            {
                acceptedHash.Add(tile.name);
            }
        }

        if(validDropTiles != null)
        {
            foreach(TileBase tile in validDropTiles)
            {
                validDropHash.Add(tile.name);
            }
        }
    }

    protected override void EntityCollisionEnter(CharacterMovement movement)
    {
        Predicate<Vector3Int> shouldReturn = (Vector3Int checkPos) => 
        {
            Vector3 worldPos = CameraMover.WorldPosForGridPos(checkPos, 0);
            TileBase tile = roomController.GetTileForWorldPos(worldPos);
            
            return tile == null || acceptedHash.Contains(tile.name);
        };

        Predicate<Vector3Int> cancelAtPos = (Vector3Int checkPos) => 
        {
            Vector3 worldPos = CameraMover.WorldPosForGridPos(checkPos, 0);
            TileBase tile = roomController.GetTileForWorldPos(worldPos);
            
            return tile != null && !validDropHash.Contains(tile.name);
        };

        posBelow = roomController.SearchInDir(myPos, searchDir, shouldReturn, cancelAtPos);
        if(myPos == posBelow)
        {
//            Debug.LogError("Ledge without position below");
            return;
        }

        if((searchDir.x != 0 && Mathf.Sign(movement.MovingDir.x) != Mathf.Sign(searchDir.x))
            || (searchDir.y != 0 && Mathf.Sign(movement.MovingDir.y) != Mathf.Sign(searchDir.y)))
        {
            return;
        }

        Vector3 pos = CameraMover.WorldPosForGridPos(posBelow, movement.transform.position.z);
        
        movement.DropdownTo(pos);
    }
    
}
