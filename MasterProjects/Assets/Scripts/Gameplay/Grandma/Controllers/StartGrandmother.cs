using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGrandmother : AnimTutorialGrandmother
{

    [SerializeField]
    private string tutorialLabel = "W A S D to move";

    public override void OnAnimationEnded() 
    {

    }
    public override bool CheckLeaveRoom(Vector3 goalPos, System.Action callback)
    { 
        balloon.HideBalloon(true);
        callback.Invoke();
        return false;
    }


}
 