using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class CharacterMovement : MonoBehaviour
{

    
    [SerializeField]
    private float moveSpeed = 1.0f;

    private Rigidbody2D body2D;

    [SerializeField]
    private CameraMoverVar cameraVar;

    private TransformVar throwTarget;

    [SerializeField]
    private GrandmaScriptVar grandmaScriptVar;
    
    [SerializeField]
    private CharacterStateVar stateVar;
    public CharacterState State => stateVar.Value;

    [SerializeField]
    private IInputController input;

    private Vector2 movingDir = Vector2.down;
    private Vector2 FacingDir => stateVar.Value == CharacterState.Walking ? movingDir : Vector2.down;


    private void Start()
    {
        body2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(stateVar.Value == CharacterState.Throwing)
        {
            if(!input.IsGrabUp())
            {
                return;
            }
            
            OnRelease();
            this.stateVar.Value = CharacterState.Idle;

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
        stateVar.Value = CharacterState.Throwing;
    }

    private void OnRelease()
    {
        
        movingDir = movingDir = Vector2.down;
        grandmaScriptVar.Value.ReleaseCharacter(this);
        stateVar.Value = CharacterState.Idle;
    }

    private void OnCry()
    {
        stateVar.Value = CharacterState.Crying;
    }

    private void OnCryCancel()
    {
        stateVar.Value = CharacterState.Idle;
    }
}

