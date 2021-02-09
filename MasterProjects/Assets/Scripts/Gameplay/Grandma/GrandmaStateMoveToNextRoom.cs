using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;
using System;

public class GrandmaStateMoveToNextRoom : GrandmaStateMoveToCall
{
    [SerializeField]
    private BoolVar isAcceptingInput;

    protected override void OnStateChange(CharacterState oldState, CharacterState newState) 
    {
    }


    protected override void OnFinishReached()
    {
        controller.OnReachedNewRoom();
        isAcceptingInput.Value = true;
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
            MoveToGoal(GetMoveGoal());
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
