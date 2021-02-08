using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class GrandmaStateMoveToCall : GrandmaState
{
    private AStarSearch<Vector2Int> grannyPath = new AStarSearch<Vector2Int>();

    [SerializeField]
    private RoomTileController roomController;

    private Vector2Int[] path;

    [SerializeField]
    private float moveSpeed = 5.0f;


    [SerializeField]
    private TransformVar characterRepresentationVar;

    private IEnumerator walkRoutine;

        
    [SerializeField]
    private RandomSelectionTextBalloonString moveToCallStrings;

    [SerializeField]
    private RandomSelectionTextBalloonString cancelCallStrings;

    [SerializeField]
    private RandomSelectionTextBalloonString cantReachStrings;

    Vector2Int currentGoal;


    protected override void OnStateChange(CharacterState oldState, CharacterState newState) 
    {
        if(newState != CharacterState.Calling)
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
    public Vector2Int[] GetPath(Vector2Int pos, Vector2Int goal)
    {
        return grannyPath.PerformSearch(pos,  goal, roomController);
    }

    public void MoveToGoal(Vector2Int goal)
    {
        Vector2Int startPos = GranGridPos;
        Vector2Int[] path = GetPath(startPos, goal);
        currentGoal = goal;

        if(path == null || path.Length == 0)
        {
            controller.Balloon.ShowText(cantReachStrings.GetRandomSelection());
            return;
        }

        walkRoutine = WalkRoutine(path);
        StartCoroutine(walkRoutine);
    }

    private IEnumerator WalkRoutine(Vector2Int[] path)
    {   
        foreach(Vector2Int nextPos in path)
        {
            if(nextPos == path[0])
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

            controller.SetMoveTarget(characterRepresentationVar.Value.position);
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
        controller.SetState(GrandmaStateEnum.Idle);
    }
}
