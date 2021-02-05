﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class CharacterMovement : MonoBehaviour
{

    
    [SerializeField]
    private float moveSpeed = 1.0f;

    private Rigidbody2D body2D;
    private Collider2D col2D;

    [SerializeField]
    private CameraMoverVar cameraVar;

    [SerializeField]
    private TransformVar throwTarget;

    [SerializeField]
    private BoolVar validThrow;

    [SerializeField]
    private GrandmaScriptVar grandmaScriptVar;
    
    [SerializeField]
    private CharacterStateVar stateVar;
    public CharacterState State => stateVar.Value;

    [SerializeField]
    private IInputController input;

    [SerializeField]
    private TransformVar grandmotherTargetVar;

    private Vector2 movingDir = Vector2.down;
    private Vector2 FacingDir => stateVar.Value == CharacterState.Walking ? movingDir : Vector2.down;


    private void Start()
    {
       body2D = GetComponent<Rigidbody2D>();
       col2D = GetComponent<Collider2D>();
    }

    void Update()
    {
        if(stateVar.Value == CharacterState.Throwing)
        {
            if(!input.IsGrabUp() || !validThrow.Value)
            {
                return;
            }
            
            OnRelease();
            this.stateVar.Value = CharacterState.Idle;

            return;
        }

        if(stateVar.Value == CharacterState.Crying)
        {
            if(input.IsCryingRelease())
            {
                OnCryCancel();
            }
            return;
        }

        if(input.IsCryingDown())
        {
            OnCry();
            return;
        }

        bool canGrab = input.IsGrabDown() 
                       && stateVar.Value != CharacterState.Throwing 
                       && grandmaScriptVar.Value.IsOnGrandma;
        if(canGrab)
        {
            OnGrab();
            return;
        }


        Vector3 dir = input.GetMovementAxis();
        if(!Mathf.Approximately(dir.magnitude, 0))
        {
            Vector3 goalPos = transform.position + moveSpeed*dir * Time.deltaTime;
            movingDir = movingDir = (Vector2)dir;
            this.stateVar.Value = CharacterState.Walking;

            body2D.MovePosition(goalPos);
        }
        else
        {
            movingDir = movingDir = Vector2.down;
            this.stateVar.Value = CharacterState.Idle;
        }
    }
    private void OnGrab()
    {
        movingDir = movingDir = Vector2.down;
        grandmaScriptVar.Value.GrabCharacter(this);
        DisableColliders();
        stateVar.Value = CharacterState.Throwing;
    }

    private void OnRelease()
    {
        
        movingDir = movingDir = Vector2.down;
        grandmaScriptVar.Value.ReleaseCharacter(this);
        EnableColliders();
        stateVar.Value = CharacterState.Idle;
    }

    private void OnCry()
    {
        stateVar.Value = CharacterState.Crying;
        
        grandmotherTargetVar.Value.transform.position = transform.position;
    }

    private void OnCryCancel()
    {
        stateVar.Value = CharacterState.Idle;
    }

    private void DisableColliders()
    {
        col2D.enabled = false;
    }

    private void EnableColliders()
    {
        col2D.enabled = true;
    }
}

