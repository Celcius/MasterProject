using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class LedgeScript : MonoBehaviour
{
    [SerializeField]
    private RoomTileController roomController;

    [SerializeField]
    private Vector2Int searchDir = new Vector2Int(0,-1);

    Vector3Int myPos;
    Vector3Int posBelow;

    private void Start()
    {
        myPos = CameraMover.GridPosForWorldPos(transform.position);
        posBelow = roomController.FindEmptyPosInDir(myPos, searchDir);
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(myPos == posBelow)
        {
            Debug.LogError("Ledge without position below");
            return;
        }

        if(other.collider.tag == GameConstants.PLAYER_TAG)
        {
            CharacterMovement movement = other.collider.GetComponent<CharacterMovement>();
            if(movement == null
               || (searchDir.x != 0 && Mathf.Sign(movement.MovingDir.x) != Mathf.Sign(searchDir.x))
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
            
            movement.JumpTo(pos);
        }
    }
}
