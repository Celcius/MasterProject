using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAnimation : RoomChangeHandler
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private string animTrigger;

    [SerializeField]
    private string cancelAnimTrigger;

    private bool hasTriggered = false;

    
    public override void OnRoomEnter(Vector2Int pos)
    {

    }

    public override void OnRoomLeave(Vector2Int pos)
    {
        if(CameraMover.RoomPosForWorldPos(transform.position) == pos)
        {
            animator.SetTrigger(cancelAnimTrigger);
            hasTriggered = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if(!hasTriggered && other.tag == GameConstants.PLAYER_TAG)
        {
            hasTriggered = true;
            animator.SetTrigger(animTrigger);
        }
    }

}
