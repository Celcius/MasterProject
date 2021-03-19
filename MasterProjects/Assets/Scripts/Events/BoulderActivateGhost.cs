using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderActivateGhost :  RoomChangeHandler
{
    [SerializeField]
    private Animator ghostAnimator;
    protected bool hasStarted = false;
    

    public override void OnRoomEnter(Vector2Int pos)
    {
        hasStarted = false;
        ghostAnimator.SetTrigger("Restart");
    }

    public override void OnRoomLeave(Vector2Int pos)
    {

    }

    protected void OnTriggerEnter2D(Collider2D other) 
    {
        if(hasStarted)
        {
            return;
        }

        Boulder boulder = other.GetComponent<Boulder>();
        if(boulder != null && !hasStarted)
        {
            hasStarted = true;  
            ghostAnimator.SetTrigger("Move");
        }
    }
}
