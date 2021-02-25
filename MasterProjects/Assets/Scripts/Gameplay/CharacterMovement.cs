using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using AmoaebaUtils;
using System;

public class CharacterMovement : MonoBehaviour
{

    
    [SerializeField]
    private float moveSpeed = 1.0f;

    private Rigidbody2D body2D;
    private Collider2D col2D;
    public Bounds ColliderBounds => col2D.bounds;

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

    private Vector2Int prevSafePos;
    public Vector2Int PrevSafePos => prevSafePos;

    [SerializeField]
    private RoomTileController roomController;

    [SerializeField]
    private JumpController throwJumpController;

    [SerializeField]
    private JumpController dropdownJumpController;

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
    
    [SerializeField]
    private BoolVar isAcceptingInput;

    private Vector2Int curRoom;

    private bool canCall;

    [SerializeField]
    private Transform cutPrefab;

    [SerializeField]
    private float cutDuration = 0.5f;
    
    [SerializeField]
    private ParticleSystem landParticles;

    [SerializeField]
    private ParticleSystem leafParticles;
    
    private ParticleSystem.EmissionModule leafEmission;

    [SerializeField]
    private CharacterRepresentation representation;
    private void Start()
    {
       gridEntity = GetComponent<GridEntity>();

       body2D = GetComponent<Rigidbody2D>();
       col2D = GetComponent<Collider2D>();
    
       roomController.AddEntityToIgnore(this.transform);
       ResetCharacter(false);

       CameraMover.Instance.OnCameraMoveStart += OnCamStart;
       CameraMover.Instance.OnCameraMoveEnd += OnCamEnd;
       leafEmission = leafParticles.emission;
       leafEmission.enabled = false;
       
       landParticles.Stop();
    }

    private void OnCamStart(Vector2Int oldPos, Vector2Int newPos)
    {
        isAcceptingInput.Value = false;
    }

    private void OnCamEnd(Vector2Int oldPos, Vector2Int newPos)
    {
        isAcceptingInput.Value = true;
        curRoom = newPos;
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
        Show();
        leafParticles.Clear();
        leafParticles.Stop();
        landParticles.Clear();
        landParticles.Stop();
        representation.OnRespawn();
    }

    void Update()
    {
        leafEmission.enabled = false;
        if(!canCall && input.IsGrabUp())
        {
            canCall = true;
        }

        if(CheckThrowing())
        {
            return;
        }

        if(CheckCut())
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

    private bool CheckCut()
    {
        if(!input.IsCutDown())
        {
            return false;
        }
        
        isAcceptingInput.Value = false;
        Transform cut = Instantiate<Transform>(cutPrefab, 
                                               representationParent.transform.position,
                                               Quaternion.identity);
        cut.right = facingDir.Value;
        StartCoroutine(StopMoveForTime(cutDuration));
        return true;
    }

    private IEnumerator StopMoveForTime(float time)
    {
        if(time > 0)
        {
            yield return new WaitForSeconds(time);
        }
        isAcceptingInput.Value = true;
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
            
            Predicate<GridEntity> ignoreGrandma = (GridEntity entity) 
                => { return entity.transform == grandmaScriptVar.Value.transform; };
            bool isEmpty = roomController.IsEmptyPos((Vector2Int)toCheckPos, ignoreGrandma);

            SetCharacterState(!isEmpty? CharacterState.Pushing : 
                                  input.IsGrab() && (canGrab && canCall)? CharacterState.Calling
                                  : CharacterState.Walking);
            

            if(roomController.IsStandablePos(GridPos, ignoreGrandma))
            {
                prevSafePos = GridPos;
            }
       

            if(IsOutOfRoom(goalPos + representationParent.localPosition))
            {
                grandmaScriptVar.Value.CheckLeaveRoom(goalPos + representationParent.localPosition, () => 
                {
                    Vector3Int goalGridPos = CameraMover.GridPosForWorldPos(goalPos+ representationParent.localPosition);
                    Vector3 nextRoomPos = CameraMover.WorldPosForGridPos(goalGridPos,0);
                    body2D.MovePosition(nextRoomPos);
                });                    
            }
            else
            {
                body2D.MovePosition(goalPos);
                TileBase tile =  roomController.GetTileForWorldPos(representationGoal);
                OnStepTile(tile);
            }
            
        }
        else
        {
            movingDir = Vector2.zero;
            SetCharacterState(input.IsGrab() && canCall? CharacterState.Calling : CharacterState.Idle);
        }      
    }

    public bool IsOutOfRoom(Vector3 pos)
    {
        Vector2Int nextRoomPos = CameraMover.RoomPosForWorldPos(pos);
        return curRoom != nextRoomPos;
    }

    private void OnGrab()
    {
        movingDir = movingDir = Vector2.down;
        grandmaScriptVar.Value.GrabCharacter(this);
        DisableColliders();
        SetCharacterState(CharacterState.Throwing);
    }

    private void OnRelease(bool isThrow)
    {
        canCall = false;
        canGrab = false;
        movingDir = movingDir = Vector2.down;
        grandmaScriptVar.Value.ReleaseCharacter(this, isThrow);

        if(!isThrow)
        {
            EnableColliders();
        }
        
        SetCharacterState(CharacterState.Idle);
    }

    private void OnStepTile(TileBase tile)
    {
        if(IsLandingTile(tile))
        {
            leafEmission.enabled = true;
        }
        else
        {
            leafEmission.enabled = false;
        }
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

    public void DisableColliders()
    {
        col2D.enabled = false;
    }

    public void EnableColliders()
    {
        col2D.enabled = true;
    }

    private void SetCharacterState(CharacterState state)
    {
        stateVar.Value = state;
    }

    public void Hide()
    {
        representationParent.gameObject.SetActive(false);
    }

    public void Show()
    {
        representationParent.gameObject.SetActive(true);
        SetCharacterState(CharacterState.Idle);
    }

    public void JumpTo(Vector3 pos)
    {
        PerformJump(throwJumpController, 
                    pos,
                    ()=> 
                    {
                        EnableColliders();
                        landParticles.Play();  
                        CameraMover.Instance.ShakeCamera(0.2f, 0.2f, 1.0f);
                    });
    }

    public void DropdownTo(Vector3 pos)
    {
        PerformJump(dropdownJumpController, 
                    pos,
                    ()=> 
                    {
                        TileBase tile = roomController.GetTileForWorldPos(pos);
                        if(IsLandingTile(tile))
                        {
                            landParticles.Play();
                        }
                        CameraMover.Instance.ShakeCamera(0.15f, 0.2f, 1.0f);
                    });
    }

    private void PerformJump(JumpController controller, Vector3 pos, Action reachCallback = null)
    {
        isAcceptingInput.Value = false;
        col2D.enabled = false;
        body2D.isKinematic = true;
        controller.JumpTo(pos, reachCallback, () => 
            { 
                col2D.enabled = true;
                body2D.isKinematic = false;
                isAcceptingInput.Value = true; 
            });
    }

    private bool IsLandingTile(TileBase tile)
    {
        return tile != null && 
                    (tile.GetType() == typeof(LandingTile) ||
                    tile.GetType().IsSubclassOf(typeof(LandingTile)) || 
                    tile.GetType() == typeof(LandingRuleTile) ||
                    tile.GetType().IsSubclassOf(typeof(LandingRuleTile)));
    }
}

