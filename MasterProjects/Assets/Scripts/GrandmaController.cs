using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class GrandmaController : MonoBehaviour
{
    [SerializeField]
    private BoolVar isOnGrandmaVar;
    public bool IsOnGrandma => isOnGrandmaVar.Value;

    [SerializeField]
    private CharacterStateVar characterStateVar;

    [SerializeField]
    private Vector2Var targetPosVar;

    [SerializeField]
    private Transform throwAnchor;
    
    private void Update()
    {
        if(isOnGrandmaVar.Value && characterStateVar.Value == CharacterState.Throwing)
        {
            transform.right = (targetPosVar.Value - (Vector2)transform.position).normalized;
        }
        else
        {
            transform.right = Vector2.right;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == GameConstants.PLAYER_TAG)
        {
            isOnGrandmaVar.Value = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.tag == GameConstants.PLAYER_TAG)
        {
            isOnGrandmaVar.Value = false;
        }
    }

    public void GrabCharacter(CharacterMovement character)
    {
        character.transform.parent = this.transform;
        character.transform.position = throwAnchor.position;
    }

    public void ReleaseCharacter(CharacterMovement character)
    {
        character.transform.parent = null;
        character.transform.right = Vector2.right;
        character.transform.position  = (Vector3)targetPosVar.Value + character.transform.position.z * Vector3.up;
    }

}
