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

    [SerializeField]
    private PushableArrVar pushing;

    private GridEntity gridEntity;

    [SerializeField]
    private Transform representationParent;

    private void Start()
    {
       gridEntity = GetComponent<GridEntity>();

       body2D = GetComponent<Rigidbody2D>();
       col2D = GetComponent<Collider2D>();
       facingDir.Value = Vector2.down;
       roomController.AddEntityToIgnore(this.transform);
    }

    void Update()
    {
        if(stateVar.Value == CharacterState.Throwing)
        {
            if(!input.IsGrabUp())
            {
                return;
            }

            bool isThrow = validThrow.Value;
            
            OnRelease(isThrow);
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
            movingDir = (Vector2)dir;
            facingDir.Value = GeometryUtils.NormalizedMaxValueVector(movingDir, false);
            
            Vector3 representationGoal = goalPos 
                                        + representationParent.localPosition 
                                        + Vector3.Scale(col2D.bounds.size, facingDir.Value)/2.0f;
            Debug.DrawLine(representationParent.position,representationGoal, Color.cyan);
            Vector3Int toCheckPos = CameraMover.GridPosForWorldPos(representationGoal);
            
            Transform[] toIgnore = new Transform[]{ grandmaScriptVar.Value.transform };
            bool isEmpty = roomController.IsEmptyPos((Vector2Int)toCheckPos, toIgnore);

            this.stateVar.Value = !isEmpty? CharacterState.Pushing : CharacterState.Walking;

            body2D.MovePosition(goalPos);
        }
        else
        {
            movingDir = Vector2.zero;
            facingDir.Value = Vector2.down;
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

    public void JumpTo(Vector3 worldPosition)
    {
        transform.position = worldPosition;
    }

    private void OnRelease(bool isThrow)
    {
        
        movingDir = movingDir = Vector2.down;
        grandmaScriptVar.Value.ReleaseCharacter(this, isThrow);
        EnableColliders();
        stateVar.Value = CharacterState.Idle;
    }

    private void OnCry()
    {
        stateVar.Value = CharacterState.Crying;
        
        grandmotherTargetVar.Value.transform.position = representationParent.transform.position;
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

