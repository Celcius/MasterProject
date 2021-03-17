using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeTutorialGrandmother : AnimTutorialGrandmother
{
    [SerializeField]
    private SoundHelperVar soundHelper;
    public void OnLand()
    {
        CameraMover.Instance.ShakeCamera(0.15f, 0.2f, 1.0f);
        ShowNextText();
        soundHelper.Value.PlaySound(GameSoundTag.SFX_LAND);
    }

    public void PlayJumpSound()
    {
        soundHelper.Value.PlaySound(GameSoundTag.SFX_JUMP);
    }

    public override void OnAnimationEnded() 
    {
        GrandmaController grandma = CreateGrandmaReplacement();
        ShowNextText(grandma.Balloon);
    }

}
