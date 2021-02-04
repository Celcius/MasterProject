using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsInputController : IInputController
{
    public override bool IsGrabDown()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    public override bool IsGrabUp()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    public override bool IsCryingDown()
    {
        return Input.GetKeyDown(KeyCode.Q);
    }

    public override bool IsCryingRelease()
    {
        return Input.GetKeyUp(KeyCode.Q);
    }

    public override Vector3 GetMovementAxis()
    {
        return new Vector3(Input.GetAxis("Horizontal"),
                           Input.GetAxis("Vertical"),
                           0);
    }

}
