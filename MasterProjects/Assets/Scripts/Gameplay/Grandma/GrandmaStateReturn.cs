using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class GrandmaStateReturn : GrandmaState
{
    [SerializeField]
    private float returnSpeed = 3.0f;

    [SerializeField]
    private RoomTileController roomTileController;


    protected override void StartBehaviour() 
    {
 
    }

    protected override void EndBehaviour() {}
    
    protected override void OnStateChange(CharacterState oldState, CharacterState newState) 
    {
        if(newState == CharacterState.Crying)
        {
            controller.SetState(GrandmaStateEnum.MoveToCry);
        }

        if(newState == CharacterState.Calling)
        {
            GrandmaStateMoveToCall moveState = (GrandmaStateMoveToCall)controller.GetStateForEnum(GrandmaStateEnum.MoveToCall);

            Vector2 halfCell = CameraMover.Instance.CellSize/2.0f;
            Vector2Int startPos = new Vector2Int(Mathf.RoundToInt(GranGridPos.x + halfCell.x),
                                             Mathf.RoundToInt(GranGridPos.y + halfCell.y));

            roomTileController.AddEntityToIgnore(controller.transform);
            Vector2Int returnGridPos = (Vector2Int)CameraMover.GridPosForWorldPos(controller.ReturnPos);
            Vector2Int[] pathToOrigin = moveState.GetPath(returnGridPos, startPos);

            Vector2Int goalPos = (Vector2Int)CameraMover.GridPosForWorldPos(controller.GetTargetPos()+(Vector3)halfCell);
            Vector2Int[] pathToCharacter = moveState.GetPath(startPos, goalPos);
            roomTileController.RemoveEntityToIgnore(controller.transform);

            bool hasPaths = (pathToOrigin != null && 
                            pathToOrigin.Length > 0)
                            && 
                            (pathToCharacter != null &&
                            pathToCharacter.Length > 0);
            if(hasPaths)
            {
                controller.StopReturn();
                controller.SetState(GrandmaStateEnum.MoveToCall);
            }

        }
    }
    
    public override bool CanGrab()
    {
        return false;
    }

    private void Update() 
    {
        float dist = Vector2.Distance(controller.transform.position, controller.ReturnPos);
        if(Mathf.Approximately(dist,0) || dist < returnSpeed*Time.deltaTime)
        {
            controller.transform.position = controller.ReturnPos;
            controller.StopReturn();
            controller.SetState(GrandmaStateEnum.Idle);
            return;
        }

        Vector2 dir = (controller.ReturnPos - controller.transform.position).normalized;
        controller.transform.position += (Vector3)dir * returnSpeed * Time.deltaTime;
        
    }

}
