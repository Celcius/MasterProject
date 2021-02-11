using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerCollideable : MonoBehaviour
{
    protected bool IsCollisionPlayer(Collider2D other, out CharacterMovement character)
    {
        if(other.tag == GameConstants.PLAYER_TAG)
        {
            character = other.GetComponent<CharacterMovement>();
            return character != null;
        }
        character = null;
        return false;
    }

    protected virtual void OnCollisionEnter2D(Collision2D other) 
    {
        CharacterMovement character;
        if(IsCollisionPlayer(other.collider, out character))
        {
           PlayerCollisionEnter(character);
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D other) 
    {
        CharacterMovement character;
        if(IsCollisionPlayer(other.collider, out character))
        {
            PlayerCollisionExit(character);
        }
    }

    protected void OnCollisionStay2D(Collision2D other) 
    {
        CharacterMovement character;
        if(IsCollisionPlayer(other.collider, out character))
        {
           PlayerCollisionStay(character);
        }
    }
    
    protected virtual void OnTriggerEnter2D(Collider2D other) 
    {
        CharacterMovement character;
        if(IsCollisionPlayer(other, out character))
        {
           PlayerTriggerEnter(character);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        CharacterMovement character;
        if(IsCollisionPlayer(other, out character))
        {
            PlayerTriggerExit(character);
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D other) 
    {
        CharacterMovement character;
        if(IsCollisionPlayer(other, out character))
        {
           PlayerTriggerStay(character);
        }
    }

    protected virtual void PlayerCollisionEnter(CharacterMovement character) {}
    protected virtual void PlayerCollisionExit(CharacterMovement character) {}
    protected virtual void PlayerCollisionStay(CharacterMovement character) {}

    protected virtual void PlayerTriggerEnter(CharacterMovement character) {}
    protected virtual void PlayerTriggerExit(CharacterMovement character) {}
    protected virtual void PlayerTriggerStay(CharacterMovement character) {}
}
