using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadGrandmaController : IGrandmaController
{
    public override Vector3 RepresentationPos => Vector3.zero;
    public override bool CanGrab  => false;
    public override bool IsOnGrandma  => false;

    public override void ResetGrandma(bool isRespawn) {}

    public override void CheckLeaveRoom(Vector3 goalPos, System.Action callback) 
    {
        this.transform.position = goalPos;
        callback?.Invoke();
    }

    public override void GrabCharacter(CharacterMovement character) {}
    public override void ReleaseCharacter(CharacterMovement character, bool throwChar) {}

    public override void OnBacktracking(){}
}
