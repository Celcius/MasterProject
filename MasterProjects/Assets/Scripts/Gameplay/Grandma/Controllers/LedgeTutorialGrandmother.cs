using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeTutorialGrandmother : AnimTutorialGrandmother
{
    [SerializeField]
    private TextBalloonString narrativeText;

    [SerializeField]
    private TextBalloonString tutorialText;

    [SerializeField]
    private TextBalloonString tutorialText2;

    [SerializeField]
    private TextBalloonString dropDownText1;
    
    [SerializeField]
    private TextBalloonString dropDownText2;

    public void ShowNarrativeText()
    {
        balloon.ShowText(narrativeText);
    }

    public void ShowTutorialText()
    {
        balloon.ShowText(tutorialText);
    }

    public void ShowTutorialText2()
    {
        balloon.ShowText(tutorialText2);
    }

    public void ShowDropDownText()
    {
        CameraMover.Instance.ShakeCamera(0.15f, 0.2f, 1.0f);
        balloon.ShowText(dropDownText1);
    }

    public void ShowDropDownText2()
    {
        balloon.ShowText(dropDownText2);
    }

    public override void OnAnimationEnded() 
    {
        GrandmaController grandma = CreateGrandmaReplacement();
        grandma.Balloon.ShowText(dropDownText2);
    }

}
