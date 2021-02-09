﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class GrandmaController : GridEntity
{
    [SerializeField]
    private BoolVar isOnGrandmaVar;
    public bool IsOnGrandma => isOnGrandmaVar.Value;

    [SerializeField]
    private CharacterStateVar characterStateVar;

    [SerializeField]
    private GridEntityVar characterVar;

    [SerializeField]
    private Vector2Var targetPosVar;

    
    [SerializeField]
    private TransformVar grandmaMoveTarget;

    [SerializeField]
    private Transform throwAnchor;

    [SerializeField]
    public SpriteRenderer representation;

    [SerializeField]
    private BoolVar canWalk;

    [SerializeField]
    private TextBalloon balloon;
    public TextBalloon Balloon => balloon;

    [SerializeField]
    private RandomSelectionTextBalloonString grabStrings;

    [SerializeField]
    private RandomSelectionTextBalloonString throwStrings;

    [SerializeField]
    private RandomSelectionTextBalloonString cancelThrowStrings;

    [SerializeField]
    private RandomSelectionTextBalloonString failLeaveStrings;

    [SerializeField]
    private RandomSelectionTextBalloonString successLeaveStrings;
 
    [SerializeField]
    private RoomTileController roomController;

    private Vector3 returnPos;
    public Vector3 ReturnPos => CameraMover.WorldPosForGridPos(CameraMover.GridPosForWorldPos(returnPos), 0);
    private bool isReturning = false;

    public bool CanGrab => !isReturning;
    
    private Dictionary<GrandmaStateEnum, GrandmaState> states = new Dictionary<GrandmaStateEnum, GrandmaState>();
    private  GrandmaState currentState;
    
    [SerializeField]
    private Vector2Var respawnPosition;

    private AStarSearch<Vector2Int> grannyPath = new AStarSearch<Vector2Int>();
    private Action onNewRoomCallback;

    protected override void Start()
    {
        GrandmaState[] grandmaStates = GetComponentsInChildren<GrandmaState>(true);
        foreach(GrandmaState state in grandmaStates)
        {
            if(states.ContainsKey(state.EnumState))
            {
                Debug.LogError("Collision on grandma state " + state.EnumState);
            }

            states[state.EnumState] = state;
        }

        SetState(GrandmaStateEnum.Idle);
        canWalk.OnChange += CanWalkChange;
        base.Start();
    }

    public void ResetGrandma(bool isRespawn)
    {
        SetState(GrandmaStateEnum.Idle);
        if(isRespawn)
        {
            transform.position = respawnPosition.Value;
        }
    }

    private void CanWalkChange(bool oldValue, bool newValue)
    {
        if(!newValue)
        {
            SetState(GrandmaStateEnum.Idle);
        }
    }

    public Vector3 GetTargetPos()
    {
        return grandmaMoveTarget.Value.transform.position;
    }

    public bool OnTarget()
    {
        return grandmaMoveTarget.Value == null || 
               CameraMover.GridPosForWorldPos(grandmaMoveTarget.Value.transform.position) 
               == CameraMover.GridPosForWorldPos(transform.position);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == GameConstants.PLAYER_TAG)
        {
            isOnGrandmaVar.Value = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.tag == GameConstants.PLAYER_TAG)
        {
            isOnGrandmaVar.Value = false;
        }
    }

    public void GrabCharacter(CharacterMovement character)
    {
        SetState(GrandmaStateEnum.Idle);
        character.transform.parent = representation.transform;
        character.transform.position = throwAnchor.position;

        balloon.ShowText(grabStrings.GetRandomSelection());
    }

    public void ReleaseCharacter(CharacterMovement character, bool throwChar)
    {
        character.transform.parent = null;
        character.transform.right = Vector2.right;
        if(throwChar)
        {
            character.transform.position  = (Vector3)targetPosVar.Value + character.transform.position.z * Vector3.up;
            balloon.ShowText(throwStrings.GetRandomSelection());
        }
        else
        {
            float angle = MathUtils.NegMod(representation.transform.rotation.eulerAngles.z, 360);
            Vector3 offset = angle > 270 || angle < 90? new Vector3(-0.5f, -0.5f, 0) : new Vector3(0.5f, -0.5f,0.5f);

            character.transform.position  = representation.transform.position + offset; 

            balloon.ShowText(cancelThrowStrings.GetRandomSelection());
        }
    }

    public void SetMoveTarget(Vector3 position)
    {
        grandmaMoveTarget.Value.transform.position = position;
    }

    public Vector2Int[] GetPath(Vector3 worldGoalPos, bool ignoreSelf = false)
    {
        Vector3Int targetGridPos = CameraMover.GridPosForWorldPos(worldGoalPos);
        return GetPath((Vector2Int)GridPos, (Vector2Int)targetGridPos, ignoreSelf);
    }

    public Vector2Int[] GetPath(Vector2Int pos, Vector2Int goal, bool ignoreSelf = false)
    {
        if(ignoreSelf)
        {
            roomController.AddEntityToIgnore(transform);
        }
        
        Vector2Int[] path = grannyPath.PerformSearch(pos,  goal, roomController);

        if(ignoreSelf)
        {
            roomController.RemoveEntityToIgnore(transform);
        }

        return path;
    }

    public void SetState(GrandmaStateEnum state)
    {
        if(currentState != null && currentState.EnumState == state)
        {
            return;
        }

        state = FilterNextEnumState(state);

        GrandmaState nextState = GetStateForEnum(state);
        if(nextState == currentState)
        {
            return;
        }

        if(currentState != null)
        {
            currentState.OnStateEnd(this);
        }
        
        currentState = nextState;
        currentState.OnStateStart(this, characterStateVar);
    }

    public void SetReturnPos(Vector3 pos)
    {
        if(isReturning)
        {
            return;
        }
        returnPos = pos;
        isReturning = true;
    }

    public void StopReturn()
    {
        isReturning = false;
    }


    private GrandmaStateEnum FilterNextEnumState(GrandmaStateEnum state)
    {
        if(isReturning && state != GrandmaStateEnum.MoveToCry)
        {
            return GrandmaStateEnum.Returning;
        }
        return state;
    }

    public GrandmaState GetStateForEnum(GrandmaStateEnum state)
    {
        if(!states.ContainsKey(state))
        {
            Debug.LogError("No instance of GrandmaState for " + state);
            return null;
        }
        return states[state];
    }

    public override void OnRoomEnter()
    {
        base.OnRoomEnter();
        ResetGrandma(false);
    }

    public override void OnRoomLeave() 
    {
        Vector3Int oldPos = GridPos;        
        Vector2Int oldRoom = CameraMover.RoomPosForGridPos(oldPos);
        Vector2Int newRoom = CameraMover.RoomPosForGridPos(characterVar.Value.GridPos);
        if(oldRoom == newRoom)
        {
            return;
        }

        this.transform.position = CameraMover.WorldPosForGridPos(characterVar.Value.GridPos, 0);
        this.originalPosition = transform.position;
        GridRegistry.Instance.ReorderRoomGridObject(this, oldRoom);
    }

    public void CheckLeaveRoom(Vector3 goalPos, Action callback)
    {
        Vector3Int gridGoalPos = CameraMover.GridPosForWorldPos(goalPos);
        Vector3 clampedworldPos = CameraMover.WorldPosForGridPos(gridGoalPos, 0);

        Vector2Int[] path = GetPath(clampedworldPos, true);

        if(gridGoalPos == GridPos || path != null && path.Length > 0)
        {
            SetMoveTarget(clampedworldPos);
            balloon.ShowText(successLeaveStrings.GetRandomSelection());
            onNewRoomCallback = callback;
            SetState(GrandmaStateEnum.MovingToNext);
        }
        else
        {
            if(!balloon.IsShowing)
            {
                balloon.ShowText(failLeaveStrings.GetRandomSelection());
            }
        }
    }
    
    public void OnReachedNewRoom()
    {
        this.originalPosition = transform.position;
        onNewRoomCallback?.Invoke();
        onNewRoomCallback = null;   
    }
}