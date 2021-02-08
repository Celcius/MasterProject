using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsInputController : IInputController
{
    [SerializeField]
    private BoolVar isAcceptingInput;
    public override bool IsGrab()
    {
        if(!isAcceptingInput.Value)
        {
            return false;
        }
        return Input.GetKeyDown(KeyCode.E) || Input.GetKey(KeyCode.E);
    }

    public override bool IsGrabUp()
    {
        if(!isAcceptingInput.Value)
        {
            return false;
        }
        return Input.GetKeyDown(KeyCode.E);
    }

    public override bool IsCryingDown()
    {
        if(!isAcceptingInput.Value)
        {
            return false;
        }
        return Input.GetKeyDown(KeyCode.Q);
    }

    public override bool IsCryingRelease()
    {
        if(!isAcceptingInput.Value)
        {
            return false;
        }
        return Input.GetKeyUp(KeyCode.Q);
    }

    public override Vector3 GetMovementAxis()
    {
        if(!isAcceptingInput.Value)
        {
            return Vector3.zero;
        }
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");
        return new Vector3(hor == 0? hor : Mathf.Sign(hor) * 1.0f,
                           ver == 0? ver : Mathf.Sign(ver) * 1.0f,
                           0);
    }

}
