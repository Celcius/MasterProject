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
    private TileBase[] tilesToIgnore;

    Vector3Int myPos;
    Vector3Int posBelow;

    private void Start()
    {
        myPos = CameraMover.GridPosForWorldPos(transform.position);
    }

    protected override void PlayerCollisionEnter(CharacterMovement movement)
    {
        posBelow = roomController.FindEmptyPosInDir(myPos, searchDir, tilesToIgnore);
        if(myPos == posBelow)
        {
            Debug.LogError("Ledge without position below");
            return;
        }

        if((searchDir.x != 0 && Mathf.Sign(movement.MovingDir.x) != Mathf.Sign(searchDir.x))
            || (searchDir.y != 0 && Mathf.Sign(movement.MovingDir.y) != Mathf.Sign(searchDir.y)))
        {
            return;
        }

        Vector3 pos = CameraMover.WorldPosForGridPos(posBelow, movement.transform.position.z);
        if(searchDir.x == 0)
        {
            pos.x = movement.transform.position.x;
        }
        else if(searchDir.y == 0)
        {
            pos.y = movement.transform.position.y;
        }
        
        movement.DropdownTo(pos);
    }
    
}
