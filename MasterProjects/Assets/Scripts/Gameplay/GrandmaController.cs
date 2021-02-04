using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class GrandmaController : GridEntity
{
    [SerializeField]
    private BoolVar isOnGrandmaVar;
    public bool IsOnGrandma => isOnGrandmaVar.Value;

    [SerializeField]
    private CharacterStateVar characterStateVar;

    [SerializeField]
    private Vector2Var targetPosVar;

    
    [SerializeField]
    private TransformVar grandmaMoveTarget;

    [SerializeField]
    private Transform throwAnchor;

    [SerializeField]
    private SpriteRenderer representation;

    private AStarSearch<Vector2Int> grannyPath = new AStarSearch<Vector2Int>();

    private IEnumerator walkRoutine;

    [SerializeField]
    private float moveSpeed = 5.0f;
    
    protected override void Start()
    {
        base.Start();
    }
    private void Update()
    {
        if(isOnGrandmaVar.Value && characterStateVar.Value == CharacterState.Throwing)
        {
            representation.transform.right = (targetPosVar.Value - (Vector2)transform.position).normalized;
        }
        else
        {
            representation.transform.right = Vector2.right;
        }

        if(walkRoutine == null && !OnTarget())
        {
            MoveToGoal((Vector2Int)CameraMover.GridPosForWorldPos(grandmaMoveTarget.Value.transform.position));
        }
    }

    private bool OnTarget()
    {
        return grandmaMoveTarget.Value == null || 
               CameraMover.GridPosForWorldPos(grandmaMoveTarget.Value.transform.position) 
               == CameraMover.GridPosForWorldPos(transform.position);
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
        StopWalking();
        character.transform.parent = representation.transform;
        character.transform.position = throwAnchor.position;
    }

    public void ReleaseCharacter(CharacterMovement character)
    {
        character.transform.parent = null;
        character.transform.right = Vector2.right;
        character.transform.position  = (Vector3)targetPosVar.Value + character.transform.position.z * Vector3.up;
    }

    
    [SerializeField]
    private RoomTileController controller;

    
    public void MoveToGoal(Vector2Int goal)
    {
        Vector2Int[] path = grannyPath.PerformSearch((Vector2Int)this.GridPos, 
                                      goal,
                                      controller);

        if(path == null || path.Length == 0)
        {
            return;
        }

        StopWalking();
        walkRoutine = WalkRoutine(path);
        StartCoroutine(walkRoutine);
                                      
    }

    private IEnumerator WalkRoutine(Vector2Int[] path)
    {   
        foreach(Vector2Int nextPos in path)
        {
            Vector3 nextWorldPos = CameraMover.WorldPosForGridPos((Vector3Int) nextPos, transform.position.z);
            float distance = Vector3.Distance(nextWorldPos, transform.position);
            Vector3 dir = (nextWorldPos - transform.position).normalized;
            
            while(distance > 0)
            {
                distance -= moveSpeed * Time.deltaTime;
                if(distance <= 0)
                {
                    transform.position = nextWorldPos;
                }
                else
                {
                    transform.position += dir* moveSpeed * Time.deltaTime;
                }
                
                yield return new WaitForEndOfFrame();
            }
        }

        walkRoutine = null;
    }

    private void StopWalking()
    {
        if(walkRoutine != null)
        {
            StopCoroutine(walkRoutine);
        }
        walkRoutine = null;
    }
}
