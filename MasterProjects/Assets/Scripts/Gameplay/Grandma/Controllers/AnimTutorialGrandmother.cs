using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimTutorialGrandmother : IGrandmaController
{
    public override Vector3 RepresentationPos => Vector3.zero;
    public override bool CanGrab  => false;
    public override bool IsOnGrandma  => false;

    private int textIndex = 0;

    [SerializeField]
    private GrandmaController grandmaPrefab;

    [SerializeField]
    protected TextBalloon balloon;

    [SerializeField]
    private TextBalloonString[] texts;

    [SerializeField]
    private RandomSelectionTextBalloonString backtrackStrings;

    private Animator animator;
    protected override void Start() 
    {
        animator = GetComponent<Animator>();
        textIndex = 0;
    }

    public override void ResetGrandma(bool isRespawn)
    {
        animator = GetComponent<Animator>();
        animator.Rebind();
        animator.Update(0f);
        textIndex = 0;
    }
    public virtual void ShowNextText()
    {
        ShowNextText(balloon);
    }

    public virtual void HideText()
    {
        balloon.HideBalloon(false);
    }

    public virtual void ShowNextText(TextBalloon balloonTarget)
    {
        if(texts == null || textIndex >= texts.Length)
        {
            return;
        }
        balloonTarget.ShowText(texts[textIndex]);
        textIndex++;
    }

    public override void CheckLeaveRoom(Vector3 goalPos, System.Action callback)
    { 

    }

    protected override void OnDestroy() 
    {
        balloon.OnHideCallback -= ReplaceGrandmother;
        base.OnDestroy();    
    }

    public override bool GrabCharacter(CharacterMovement character) { return false; }

    public override void ReleaseCharacter(CharacterMovement character, bool throwChar) {}

    public virtual void OnAnimationEnded() 
    {
        if(balloon.IsShowing)
        {
            balloon.OnHideCallback += ReplaceGrandmother;
            balloon.HideBalloon(false);
        }
        else
        {
            ReplaceGrandmother();
        }

    }

    protected void ReplaceGrandmother()
    {
        balloon.OnHideCallback -= ReplaceGrandmother;
        CreateGrandmaReplacement();
    }

    protected GrandmaController CreateGrandmaReplacement()
    {
        GrandmaController grandma = Instantiate<GrandmaController>(grandmaPrefab, transform.position, transform.rotation);
        Destroy(this.gameObject);
        return grandma;
    }

    public override void OnBacktracking()
    {
        if(balloon.IsLeavingOrHidden && backtrackStrings != null)
        {
            balloon.ShowText(backtrackStrings.GetRandomSelection());
        }
    }
}
