using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IInputController : ScriptableObject
{
    public abstract bool IsGrab();
    public abstract bool IsGrabUp();
    public abstract bool IsCryingDown();
    public abstract bool IsCutDown();
    public abstract bool IsCryingRelease();
    public abstract Vector3 GetMovementAxis();

    public abstract bool IsNextRoomDown();
    public abstract bool IsPrevRoomDown();
}
