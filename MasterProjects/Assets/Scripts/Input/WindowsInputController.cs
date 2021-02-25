using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsInputController : IInputController
{
    private const KeyCode grabKey = KeyCode.Q;
    private const KeyCode cutKey = KeyCode.E;
    private const KeyCode cryKey = KeyCode.Escape;
    private const KeyCode specialKey = KeyCode.LeftShift;
    private const KeyCode nextRoomKey = KeyCode.RightArrow;
    private const KeyCode prevRoomKey = KeyCode.LeftArrow;

    [SerializeField]
    private BoolVar canCut;

    [SerializeField]
    private BoolVar isAcceptingInput;
    public override bool IsGrab()
    {
        if(!isAcceptingInput.Value)
        {
            return false;
        }
        return Input.GetKeyDown(grabKey) || Input.GetKey(grabKey);
    }

    public override bool IsGrabUp()
    {
        if(!isAcceptingInput.Value)
        {
            return false;
        }
        return Input.GetKeyDown(grabKey);
    }

    public override bool IsCutDown()
    {
        if(!isAcceptingInput.Value || !canCut.Value)
        {
            return false;
        }

        return Input.GetKeyDown(cutKey);
    }


    public override bool IsCryingDown()
    {
        if(!isAcceptingInput.Value)
        {
            return false;
        }
        return Input.GetKeyDown(cryKey);
    }

    public override bool IsCryingRelease()
    {
        if(!isAcceptingInput.Value)
        {
            return false;
        }
        return Input.GetKeyUp(cryKey);
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

    public override bool IsPrevRoomDown()
    {
        if(!isAcceptingInput.Value)
        {
            return false;
        }
        return Input.GetKey(specialKey) && Input.GetKeyUp(prevRoomKey);
    }

    public override bool IsNextRoomDown()
    {
        if(!isAcceptingInput.Value)
        {
            return false;
        }
        return Input.GetKey(specialKey) && Input.GetKeyDown(nextRoomKey);
    }


}
