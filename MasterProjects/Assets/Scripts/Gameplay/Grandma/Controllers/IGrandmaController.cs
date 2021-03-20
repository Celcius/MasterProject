using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IGrandmaController : GridEntity
{
    public virtual Vector3 RepresentationPos {get;}
    public virtual bool CanGrab {get;}
    public virtual bool IsOnGrandma  {get;}

    public abstract void ResetGrandma(bool isRespawn);

    public abstract bool CheckLeaveRoom(Vector3 goalPos, System.Action callback);

    public abstract bool GrabCharacter(CharacterMovement character);
    public abstract void ReleaseCharacter(CharacterMovement character, bool throwChar);

    public abstract void OnBacktracking();

    public virtual bool IsGrandmaDead()
    {
        return false;
    }

    public abstract TextBalloon GetBalloon();
}
