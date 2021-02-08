using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
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
    public Vector2 MovingDir => movingDir;
    [SerializeField]
    private Vector2Var facingDir;

    [SerializeField]
    private RoomTileController roomController;

    private GridEntity gridEntity;
    public Vector2Int GridPos 
    {
        get 
        {
            return (Vector2Int)CameraMover.GridPosForWorldPos(representationParent.transform.position);
        }
    }

    [SerializeField]
    private Transform representationParent;

    [SerializeField]
    private Vector2Var respawnPosition;

    private bool canGrab = true;

    private void Start()
    {
       gridEntity = GetComponent<GridEntity>();

       body2D = GetComponent<Rigidbody2D>();
       col2D = GetComponent<Collider2D>();
       roomController.AddEntityToIgnore(this.transform);
       ResetCharacter(false);
    }

    public void ResetCharacter(bool isRespawn)
    {
        facingDir.Value = Vector2.down;
        movingDir = Vector2.down;
        SetCharacterState(CharacterState.Idle);
        if(isRespawn)
        {
            transform.position = respawnPosition.Value;
        }
    }

    void Update()
    {
        if(CheckThrowing())
        {
            return;
        }

        if(CheckCrying())
        {
            return;
        }

        if(CheckGrab())
        {
            return;
        }
    
        HandleMove();
    }

    private bool CheckThrowing()
    {
        if(stateVar.Value == CharacterState.Throwing)
        {
            if(!input.IsGrabUp())
            {
                return true;
            }

            bool isThrow = validThrow.Value;
            
            OnRelease(isThrow);
            SetCharacterState(CharacterState.Idle);

            return true;
        }
        return false;
    }

    private bool CheckCrying()
    {
        if(stateVar.Value == CharacterState.Crying)
        {
            if(input.IsCryingRelease())
            {
                OnCryCancel();
            }
            return true;
        }

        if(input.IsCryingDown())
        {
            OnCry();
            return true;
        }
        return false;
    }

    private bool CheckGrab()
    {
        /*
        if(stateVar.Value == CharacterState.Calling && !(input.IsGrab() || input.IsGrabDown()))
        {
            OnCallCancel();
        }
        */
        if(!canGrab)
        {
            canGrab |= input.IsGrabUp();
        }


        if(canGrab && input.IsGrab())
        {
            if(grandmaScriptVar.Value.IsOnGrandma && grandmaScriptVar.Value.CanGrab)
            {
                if(stateVar.Value != CharacterState.Throwing)
                {
                    OnGrab();
                }
                return true;
            }
            else if(!grandmaScriptVar.Value.IsOnGrandma 
                    && stateVar.Value != CharacterState.Calling
                    && stateVar.Value != CharacterState.Throwing)
            {
                OnCallGrandmother();
            }
        }
        return false;
    }

    private void HandleMove()
    {        
        Vector3 dir = input.GetMovementAxis();
        
        if(!Mathf.Approximately(dir.magnitude, 0))
        {
            Vector3 goalPos = transform.position + moveSpeed*dir * Time.deltaTime;
            movingDir = (Vector2)dir;
            facingDir.Value = GeometryUtils.NormalizedMaxValueVector(movingDir, false);
            
            Vector3 representationGoal = goalPos 
                                        + representationParent.localPosition 
                                        + Vector3.Scale(col2D.bounds.size, facingDir.Value)/2.0f;
            Debug.DrawLine(representationParent.position,representationGoal, Color.cyan);
            Vector3Int toCheckPos = CameraMover.GridPosForWorldPos(representationGoal);
            
            Transform[] toIgnore = new Transform[]{ grandmaScriptVar.Value.transform };
            bool isEmpty = roomController.IsEmptyPos((Vector2Int)toCheckPos, toIgnore);

            SetCharacterState(!isEmpty? CharacterState.Pushing : 
                                  input.IsGrab() && canGrab? CharacterState.Calling
                                  : CharacterState.Walking);

            body2D.MovePosition(goalPos);
        }
        else
        {
            movingDir = Vector2.zero;
            facingDir.Value = Vector2.down;
            SetCharacterState(input.IsGrab()? CharacterState.Calling : CharacterState.Idle);
        }      
    }

    private void OnGrab()
    {
        movingDir = movingDir = Vector2.down;
        grandmaScriptVar.Value.GrabCharacter(this);
        DisableColliders();
        SetCharacterState(CharacterState.Throwing);
    }

    public void JumpTo(Vector3 worldPosition)
    {
        transform.position = worldPosition;
    }

    private void OnRelease(bool isThrow)
    {
        canGrab = false;
        movingDir = movingDir = Vector2.down;
        grandmaScriptVar.Value.ReleaseCharacter(this, isThrow);
        EnableColliders();
        SetCharacterState(CharacterState.Idle);
    }

    private void OnCry()
    {
        SetCharacterState(CharacterState.Crying);
    }

    private void OnCryCancel()
    {
        SetCharacterState(CharacterState.Idle);
    }

    private void OnCallGrandmother()
    {
        SetCharacterState(CharacterState.Calling);
    }

    private void OnCallCancel()
    {
        SetCharacterState(CharacterState.Idle);
    }

    private void DisableColliders()
    {
        col2D.enabled = false;
    }

    private void EnableColliders()
    {
        col2D.enabled = true;
    }

    private void SetCharacterState(CharacterState state)
    {
        stateVar.Value = state;
    }
}

