using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class GrandmaStateMoveToCry : GrandmaState
{

    [SerializeField]
    private float moveSpeed = 10.0f;

    
    [SerializeField]
    private float stopDistance = 2.0f;

    [SerializeField]
    private TransformVar characterRepresentation;
    
    [SerializeField]
    private RandomSelectionTextBalloonString moveToCryStrings;

    [SerializeField]
    private RandomSelectionTextBalloonString cancelCryStrings;

    public override bool CanGrab()
    {
        return false;
    }

    protected override void OnStateChange(CharacterState oldState, CharacterState newState) 
    {
        if(newState != CharacterState.Crying)
        {
            controller.SetState(GrandmaStateEnum.Idle);
        }
    }

    protected override void StartBehaviour()
    {
        controller.SetReturnPos(GranPos);
        controller.Balloon.ShowText(moveToCryStrings.GetRandomSelection());
    }

    protected override void EndBehaviour()
    {
        controller.Balloon.ShowText(cancelCryStrings.GetRandomSelection());
    }

    private void Update() 
    {
        if(Vector2.Distance(GranPos, characterRepresentation.Value.position) < stopDistance)
        {
            return;
        }

        Vector2 dir = (characterRepresentation.Value.position - GranPos).normalized;
        controller.transform.position += (Vector3)dir * moveSpeed * Time.deltaTime;
    }
}
