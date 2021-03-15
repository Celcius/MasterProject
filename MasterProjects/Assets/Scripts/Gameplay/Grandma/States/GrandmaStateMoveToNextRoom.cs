using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;
using System;

public class GrandmaStateMoveToNextRoom : GrandmaStateMoveToCall
{
    [SerializeField]
    private BoolVar isAcceptingInput;

    [SerializeField]
    private RoomTileController roomTileController;

    protected override void OnStateChange(CharacterState oldState, CharacterState newState) 
    {
    }


    protected override void OnFinishReached(Vector2Int oldRoomPos)
    {
        controller.OnReachedNewRoom(oldRoomPos);
    }

    protected override void StartBehaviour()
    {
        updateToCharacter = false;
        if(controller.OnTarget())
        {
            controller.SetState(GrandmaStateEnum.Idle);
            return;
        }
        else
        {
            isAcceptingInput.Value = false;
            roomTileController.GenerateBorderNeighbours = true;
            MoveToGoal(GetMoveGoal());
            roomTileController.GenerateBorderNeighbours = false;
        }
    }
    
    protected override void EndBehaviour()
    {        
        controller.SetMoveTarget(GranPos);

        if(walkRoutine != null)
        {   
            StopCoroutine(walkRoutine);
        }
        walkRoutine = null;
    }
}
