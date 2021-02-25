using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCollider : RoomChangeHandler
{
    [SerializeField]
    private string tutorialString;
    
    [SerializeField]
    private StringVar tutorialVar;
    protected bool hasShown = false;
    
    protected virtual void OnTriggerEnter2D(Collider2D other) 
    {
        CharacterMovement character;
        if(PlayerCollideable.IsCollisionPlayer(other, out character))
        {
           PlayerTriggerEnter(character);
        }
    }

    protected virtual void PlayerTriggerEnter(CharacterMovement character) 
    {
        if(hasShown)
        {
            return;
        }
        tutorialVar.Value = tutorialString;
        hasShown = true;

    }

    public override void OnRoomEnter(Vector2Int pos)
    {
        hasShown = false;
    }

    public override void OnRoomLeave(Vector2Int pos)
    {

    }
}
