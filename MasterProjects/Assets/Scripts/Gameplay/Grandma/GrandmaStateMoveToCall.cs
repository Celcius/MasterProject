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


    protected override void OnStateChange(CharacterState oldState, CharacterState newState) 
    {
        if(newState != CharacterState.Calling)
        {
            controller.SetState(GrandmaStateEnum.Idle);
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
        Vector2 halfCell = CameraMover.Instance.CellSize/2.0f;
        Vector2Int startPos = new Vector2Int(Mathf.RoundToInt(GranGridPos.x + halfCell.x),
                                             Mathf.RoundToInt(GranGridPos.y + halfCell.y));
        Vector2Int[] path = GetPath(startPos, goal);

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
        }
        
        walkRoutine = null;
        controller.SetState(GrandmaStateEnum.Idle);
    }
}
