﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrannyHi5 : GrandmaController
{
    
    public override bool CanGrab => !hasHi5;

    [SerializeField]
    private IGrandmaController grandmaPrefab;

    [SerializeField]
    private TextBalloonString onMoveText;

    [SerializeField]
    private float minTimeToShowOnMoveText = 2.0f;

    [SerializeField]
    private TextBalloonString[] onArrive;

    [SerializeField]
    private float[] onArriveDuration;

    [SerializeField]
    private RandomSelectionTextBalloonString leaveAttemptStrings;

    private bool canHi5 = false;
    private bool hasHi5 = false;

    private IEnumerator interactRoutine;

    [SerializeField]
    private Vector3 goalPos;

    [SerializeField]
    private Vector3 characterGoalPos;


    [SerializeField]
    private float speed = 7;
    
    [SerializeField]
    private StringVar tutorialStringVar;

    [SerializeField]
    private string tutorialString;

    [SerializeField]
    private BoolVar isAcceptingInput;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Transform animCharacterAnchor;

    private Transform cachedParent;
    private CharacterMovement highFiveChar;

    [SerializeField]
    private ParticleSystem[] particles;

    [SerializeField]
    private TextBalloonString onHighFiveEnd;

    [SerializeField]
    private TextBalloonString goalBlockedText;

    [SerializeField]
    private StringVar heightIndicatorVar;

    [SerializeField]
    private string heightString;

    private float frameAtStart;


    protected override void Start() 
    {
        base.Start();
        ResetGrandma(false);
        animator = GetComponent<Animator>();
    }

    public override void ResetGrandma(bool isRespawn)
    {
        base.ResetGrandma(isRespawn);
        canHi5 = false;
        hasHi5 = false;
        if(interactRoutine != null)
        {
            StopCoroutine(interactRoutine);
            interactRoutine = null;
        }

        GrandmaStateIdle idle = (GrandmaStateIdle)GetStateForEnum(GrandmaStateEnum.Idle);
        idle.CanMoveToPlayer = false;

        frameAtStart = Time.time;

        interactRoutine = StartRoutine();
        StartCoroutine(interactRoutine);
    }

    private IEnumerator StartRoutine()
    {
        Vector2Int[]path = null;

        bool shownBlockedText = false;
        do 
        {
            yield return new WaitForEndOfFrame();
            path = GetPath(goalPos, true);

            if(!shownBlockedText 
               && !string.IsNullOrEmpty(goalBlockedText.textString) 
               && (path  == null || path.Length <= 1))
            {
                Balloon.ShowText(goalBlockedText, false);
                shownBlockedText = true;
            }
        }
        while(path == null || path.Length <= 1);

        
        Balloon.ShowText(onMoveText, false);
        yield return SimpleWalkRoutine(path, speed, OnReachCallback);
        
    }

    private void OnReachCallback()
    {
        interactRoutine = TalkRoutine();
        StartCoroutine(interactRoutine);
    }

    private IEnumerator TalkRoutine()
    {
        while(Time.time - frameAtStart < minTimeToShowOnMoveText)
        {
            yield return new WaitForEndOfFrame();
        }
        
        for(int i = 0; i < onArrive.Length; i++)
        {
            if(i == 0)
            {
                heightIndicatorVar.Value = heightString;
            }

            TextBalloonString arriveStr = onArrive[i];
            Balloon.ShowText(arriveStr);
            yield return new WaitForSeconds(onArriveDuration[i]);
        }

        canHi5 = true;
        tutorialStringVar.Value = tutorialString;
    }


    [SerializeField]
    public override void ReleaseCharacter(CharacterMovement character, bool throwChar)
    {
        base.ReleaseCharacter(character, throwChar);
    }

    [SerializeField]
    public override bool GrabCharacter(CharacterMovement character)
    {
        if(!canHi5)
        {
            return false;
        }

        if(!hasHi5)
        {
            PerformHi5(character);
            return false;
        }
        return base.GrabCharacter(character);
    }

    public override void OnBacktracking()
    {
        if(!hasHi5)
        {
            if(Balloon.IsLeavingOrHidden)
            {
                Balloon.ShowText(leaveAttemptStrings.GetRandomSelection());
            }
            return;
        }
        base.OnBacktracking();
    }

    public override bool CheckLeaveRoom(Vector3 goalPos, System.Action callback)
    {
        if(!hasHi5)
        {
            if(Balloon.IsLeavingOrHidden)
            {
                Balloon.ShowText(leaveAttemptStrings.GetRandomSelection());
            }
            return false;
        }
        return base.CheckLeaveRoom(goalPos, callback);
    }

    private void PerformHi5(CharacterMovement character)
    {   
        soundHelper.Value.MusicPlayer.PlayStyle(MusicPlayer.MusicStyle.GoldenTime);
        soundHelper.Value.SoundSystem.TransitionToSnapshot("Hi5SnapShot", 0.25f);
;
        Balloon.HideBalloon(false);
        cachedParent = character.transform.parent;
        character.transform.parent = animCharacterAnchor;
        highFiveChar = character;

        canHi5 = false;
        isAcceptingInput.Value = false;
        character.DisableColliders();

        Vector3[]path = new Vector3[] {character.transform.position, characterGoalPos};

        interactRoutine = SimpleWalkRoutine(path, speed, OnPlayerReach, character.transform);
        StartCoroutine(interactRoutine);
    }

    private void OnPlayerReach()
    {
        soundHelper.Value.PlaySound(GameSoundTag.SFX_HI5_CLAP);
        highFiveChar.transform.parent = animCharacterAnchor;
        animator.SetTrigger("HighFive");
    }

    public void HighFiveEnd()
    {
        
        soundHelper.Value.SoundSystem.TransitionToSnapshot("Default", 1.0f);
        highFiveChar.EnableColliders();
        highFiveChar.transform.parent = cachedParent;
        highFiveChar = null;
        cachedParent = null;
        
        GrandmaStateIdle idle = (GrandmaStateIdle)GetStateForEnum(GrandmaStateEnum.Idle);
        idle.CanMoveToPlayer = true;   
        hasHi5 = true;
        isAcceptingInput.Value = true;

        IGrandmaController grandma = Instantiate<IGrandmaController>(grandmaPrefab, transform.position, transform.rotation);
        TextBalloon balloon = grandma.GetBalloon();
        if(balloon != null)
        {
            balloon.ShowText(onHighFiveEnd, false, false);
        }
        
        Destroy(gameObject);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if(highFiveChar != null)
        {
            highFiveChar.transform.parent = cachedParent;
        }
    }

    public void HighFiveHit()
    {
        foreach(ParticleSystem system in particles)
        {
            system.Clear();
            system.Play();
        }
    }
}
 