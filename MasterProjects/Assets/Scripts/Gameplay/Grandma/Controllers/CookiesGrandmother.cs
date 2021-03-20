using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CookiesGrandmother : AnimTutorialGrandmother
{
    [SerializeField]
    private GridEntityVar child;

    [SerializeField]
    private Vector3 childWorldPos;
    
    [SerializeField]
    private Vector3 hamperWorldPos;

    [SerializeField]
    private BoolVar inputActive;

    [SerializeField]
    private Transform hamper;

    private Transform hamperTransform = null;

    [SerializeField]
    private SoundHelperVar soundHelper;

    [SerializeField]
    private AudioMixerSnapshot eatSnapShot;

    [SerializeField]
    private AudioMixerSnapshot defaultSnapShot;

    public void ActivateInput()
    {
        inputActive.Value = true;
    }

    public void PlayDefaultSnapshot()
    {
        defaultSnapShot.TransitionTo(3.0f);
    }
    public void PlayEatSnapshot()
    {
        eatSnapShot.TransitionTo(1.0f);
    }

    public void DeactivateInput()
    {
        inputActive.Value = false;
    }

    public void PlaceChild()
    {
        child.Value.transform.position = childWorldPos;
    }

    public void PlaceHamper()
    {
        hamperTransform = Instantiate(hamper, hamperWorldPos, Quaternion.identity);
    }

    public void RemoveHamper()
    {
        Destroy(hamperTransform.gameObject);
    }

    public void PlayEatSounds()
    {
        eatSnapShot.TransitionTo(0.0f);
        soundHelper.Value.PlaySound(GameSoundTag.SFX_EAT_SCENE);
    }

    protected override void OnDestroy() 
    {
        if(hamperTransform != null)
        {
            Destroy(hamperTransform.gameObject);
        }  
        defaultSnapShot.TransitionTo(0.0f);

        base.OnDestroy();    
    }

    
    public override void OnAnimationEnded() 
    {
        GrandmaController nana = CreateGrandmaReplacement();
        balloon.OnHideCallback -= ReplaceGrandmother;

        nana.Balloon.ShowText(texts[textIndex]);
    }
}
