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
    HashSet<string> ignoreHash = new HashSet<string>();

    private void Start()
    {
        myPos = CameraMover.GridPosForWorldPos(transform.position);
        if(tilesToIgnore != null)
        {
            foreach(TileBase tile in tilesToIgnore)
            {
                ignoreHash.Add(tile.name);
            }
        }
    }

    protected override void PlayerCollisionEnter(CharacterMovement movement)
    {
        System.Predicate<TileBase> toIgnorePredicate = (TileBase tile) => 
        {
            return ignoreHash.Contains(tile.name);
        };

        posBelow = roomController.FindEmptyPosInDir(myPos, searchDir, toIgnorePredicate);
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
        
        movement.DropdownTo(pos);
    }
    
}
