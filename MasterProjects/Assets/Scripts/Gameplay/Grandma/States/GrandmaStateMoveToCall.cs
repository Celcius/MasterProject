using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class GrandmaStateMoveToCall : GrandmaState
{

    private Vector2Int[] path;

    [SerializeField]
    private float moveSpeed = 5.0f;


    [SerializeField]
    protected TransformVar characterRepresentationVar;

    protected IEnumerator walkRoutine;

        
    [SerializeField]
    private RandomSelectionTextBalloonString moveToCallStrings;

    [SerializeField]
    private RandomSelectionTextBalloonString cancelCallStrings;

    [SerializeField]
    private RandomSelectionTextBalloonString cantReachStrings;

    Vector2Int currentGoal;

    protected bool updateToCharacter = true;

    protected override void OnStateChange(CharacterState oldState, CharacterState newState) 
    {
        if(newState == CharacterState.Idle
           || newState == CharacterState.Walking
           || newState == CharacterState.Pushing)
        {
            return;
        } 
        else if(newState == CharacterState.Crying)
        {
            controller.SetState(GrandmaStateEnum.MoveToCry);
        } 
        else if(newState != CharacterState.Calling)
        {
            controller.SetState(GrandmaStateEnum.Idle);
        }
    }

    private void FixedUpdate() 
    {
        if(walkRoutine == null)
        {
            controller.SetMoveTarget(characterRepresentationVar.Value.position);
            Vector2Int curGoal = GetMoveGoal();
            if(currentGoal != curGoal)
            {
                MoveToGoal(curGoal);
            }
        }    
    }

    protected override void StartBehaviour()
    {
        controller.SetMoveTarget(characterRepresentationVar.Value.position);
        
        if(controller.OnTarget())
        {
            controller.SetState(GrandmaStateEnum.Idle);
            return;
        }
        else
        {
            controller.Balloon.ShowText(moveToCallStrings.GetRandomSelection());
            MoveToGoal(GetMoveGoal());
        }
    }

    protected override void EndBehaviour()
    {
        if(characterState.Value != CharacterState.Throwing)
        {
            controller.Balloon.ShowText(cancelCallStrings.GetRandomSelection());
        }
        
        controller.SetMoveTarget(GranPos);

        if(walkRoutine != null)
        {   
            StopCoroutine(walkRoutine);
        }
        walkRoutine = null;
    }

    public Vector2Int GetMoveGoal()
    {
        return (Vector2Int)CameraMover.GridPosForWorldPos(TargetPos);
    }

    public void MoveToGoal(Vector2Int goal)
    {
        Vector2Int startPos = GranGridPos;
        Vector2Int[] path = controller.GetPath(startPos, goal);
        currentGoal = goal;

        if(path == null || path.Length == 0)
        {
            controller.Balloon.ShowText(cantReachStrings.GetRandomSelection());
            controller.SetState(GrandmaStateEnum.Idle);
            return;
        }

        walkRoutine = WalkRoutine(path);
        StartCoroutine(walkRoutine);
    }

    protected virtual void OnFinishReached(Vector2Int oldRoomPos)
    {

    }

    private IEnumerator WalkRoutine(Vector2Int[] path)
    {   
        Vector2Int currentRoomPos = CameraMover.RoomPosForWorldPos(transform.position);
        foreach(Vector2Int nextPos in path)
        {
            if(nextPos == path[0] || GranGridPos == nextPos)
            {
                continue;
            }

            Vector3 nextWorldPos = CameraMover.WorldPosForGridPos((Vector3Int) nextPos, controller.transform.position.z);
            float distance = Vector3.Distance(nextWorldPos, controller.transform.position);
            Vector3 dir = (nextWorldPos - controller.transform.position).normalized;
            
            while(distance > 0)
            {
                distance -= moveSpeed * Time.deltaTime;
                if(distance <= 0)
                {
                    controller.transform.position = nextWorldPos;
                }
                else
                {
                    controller.transform.position += dir* moveSpeed * Time.deltaTime;
                }
                
                yield return new WaitForEndOfFrame();
            }

            if(updateToCharacter)
            {
                controller.SetMoveTarget(characterRepresentationVar.Value.position);
            }

            Vector2Int curGoal = GetMoveGoal();
            if(curGoal != currentGoal)
            {
                StopCoroutine(walkRoutine);
                walkRoutine = null;
                MoveToGoal(curGoal);
                yield break;
            }
        }

        walkRoutine = null;
        OnFinishReached(currentRoomPos);
        controller.SetState(GrandmaStateEnum.Idle);
    }
}
