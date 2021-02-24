using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimTutorialGrandmother : IGrandmaController
{
    public override Vector3 RepresentationPos => Vector3.zero;
    public override bool CanGrab  => false;
    public override bool IsOnGrandma  => false;

    [SerializeField]
    private GrandmaController grandmaPrefab;

    [SerializeField]
    protected TextBalloon balloon;


    private Animator animator;
    protected override void Start() 
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    public override void ResetGrandma(bool isRespawn)
    {
        animator = GetComponent<Animator>();
        animator.Rebind();
        animator.Update(0f);
    }

    public override void CheckLeaveRoom(Vector3 goalPos, System.Action callback)
    { 

    }

    protected override void OnDestroy() 
    {
        balloon.OnHideCallback -= ReplaceGrandmother;
        base.OnDestroy();    
    }

    public override void GrabCharacter(CharacterMovement character) {}

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
}
