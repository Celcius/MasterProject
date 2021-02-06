using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerCollideable : MonoBehaviour
{
    protected bool IsCollisionPlayer(Collision2D other, out CharacterMovement character)
    {
        if(other.collider.tag == GameConstants.PLAYER_TAG)
        {
            character = other.collider.GetComponent<CharacterMovement>();
            return character != null;
        }
        character = null;
        return false;
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        CharacterMovement character;
        if(IsCollisionPlayer(other, out character))
        {
           PlayerCollisionEnter(character);
        }
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
        CharacterMovement character;
        if(IsCollisionPlayer(other, out character))
        {
            PlayerCollisionExit(character);
        }
    }

    private void OnCollisionStay2D(Collision2D other) 
    {
        CharacterMovement character;
        if(IsCollisionPlayer(other, out character))
        {
           PlayerCollisionStay(character);
        }
    }
    
    protected virtual void PlayerCollisionEnter(CharacterMovement character) {}
    protected virtual void PlayerCollisionExit(CharacterMovement character) {}
    protected virtual void PlayerCollisionStay(CharacterMovement character) {}
}
