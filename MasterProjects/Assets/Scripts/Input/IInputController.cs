using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IInputController : ScriptableObject
{
    public abstract bool IsGrabDown();
    public abstract bool IsGrabUp();
    public abstract bool IsCryingDown();

    public abstract bool IsCryingRelease();
    public abstract Vector3 GetMovementAxis();
}
