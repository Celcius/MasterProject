using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class GrandmaStateIdle : GrandmaState
{
    [SerializeField]
    private Vector2Var targetPosVar;

    public bool CanMoveToPlayer = true;
    protected override void StartBehaviour()
    {
        
    }

    protected override void EndBehaviour()
    {

    }

    protected override void OnStateChange(CharacterState oldState, CharacterState newState) 
    {
        
        if(newState == CharacterState.Calling && CanMoveToPlayer)
        {
            controller.SetState(GrandmaStateEnum.MoveToCall);
            return;
        }

        if(newState == CharacterState.Crying && CanMoveToPlayer)
        {
            controller.SetState(GrandmaStateEnum.MoveToCry);
            return;
        }

    }

    public override bool CanGrab()
    {
        return characterState.Value != CharacterState.Throwing;
    }

    private void Update()
    {
        if(characterState.Value == CharacterState.Throwing)
        {
            Representation.transform.right = (targetPosVar.Value - (Vector2)GranPos).normalized;
        }
        else
        {
            Representation.transform.right = Vector2.right;
        }
    }
}
