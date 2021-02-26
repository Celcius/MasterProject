using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeTutorialGrandmother : AnimTutorialGrandmother
{
    public void OnLand()
    {
        CameraMover.Instance.ShakeCamera(0.15f, 0.2f, 1.0f);
        ShowNextText();
    }
    public override void OnAnimationEnded() 
    {
        GrandmaController grandma = CreateGrandmaReplacement();
        ShowNextText(grandma.Balloon);
    }

}
