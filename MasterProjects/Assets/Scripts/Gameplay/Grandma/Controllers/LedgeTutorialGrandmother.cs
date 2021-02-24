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
        balloon.ShowText(dropDownText1);
    }

    public void ShowDropDownText2()
    {
        balloon.ShowText(dropDownText2);
    }
}
