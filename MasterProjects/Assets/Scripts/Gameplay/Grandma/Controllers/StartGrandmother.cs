using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGrandmother : AnimTutorialGrandmother
{

    [SerializeField]
    private string tutorialLabel = "W A S D to move";

    [SerializeField]
    private StringVar tutorialStringVar;

    public void ShowTutorialString()
    {
        tutorialStringVar.Value = tutorialLabel;
    }
    public override void OnAnimationEnded() 
    {

    }
    public override void CheckLeaveRoom(Vector3 goalPos, System.Action callback)
    { 
        balloon.HideBalloon(true);
        callback.Invoke();
    }


}
 