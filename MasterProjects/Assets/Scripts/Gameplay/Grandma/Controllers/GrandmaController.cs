using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

[RequireComponent(typeof(GrandmaPathFeeder))]
public class GrandmaController : IGrandmaController
{
    [SerializeField]
    protected BoolVar isOnGrandmaVar;
    public override bool IsOnGrandma => isOnGrandmaVar.Value;

    [SerializeField]
    private CharacterStateVar characterStateVar;

    [SerializeField]
    private Collider2D physicalCollider;

    [SerializeField]
    protected GridEntityVar characterVar;

    [SerializeField]
    private Vector2Var targetPosVar;

    
    [SerializeField]
    private TransformVar grandmaMoveTarget;

    [SerializeField]
    private Transform throwAnchor;

    [SerializeField]
    public SpriteRenderer representation;
    public override Vector3 RepresentationPos => representation.transform.position;

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
    private RandomSelectionTextBalloonString backtrackStrings;
 
    [SerializeField]
    private RoomTileController roomController;

    [SerializeField]
    private float maxDropDistance = 0.5f;

    [SerializeField]
    protected SoundHelperVar soundHelper;

    private GrandmaPathFeeder grandmaPathFeeder;

    private Vector3 returnPos;
    public Vector3 ReturnPos => CameraMover.WorldPosForGridPos(CameraMover.GridPosForWorldPos(returnPos), 0);
    private bool isReturning = false;

    public override bool CanGrab => !isReturning;
    
    private Dictionary<GrandmaStateEnum, GrandmaState> states = new Dictionary<GrandmaStateEnum, GrandmaState>();
    private  GrandmaState currentState;
    
    [SerializeField]
    private Vector2Var respawnPosition;

    private AStarSearch<Vector2Int> grannyPath = new AStarSearch<Vector2Int>();
    private Action onNewRoomCallback;

    private Vector2Int currentRoom;
    private bool hasSetup = false;
    protected override void Start()
    {
        Setup();
        base.Start();
    }

    private void Setup()
    {
        if(hasSetup)
        {
            return;
        }
        grandmaPathFeeder = GetComponent<GrandmaPathFeeder>();
        states.Clear();
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
        hasSetup = true;
    }

    protected override void OnDestroy() 
    {
        if(currentState != null)
        {
            currentState.OnStateEnd(this);
        }
        base.OnDestroy();
    }
    public override void ResetGrandma(bool isRespawn)
    {
        Setup();
        SetState(GrandmaStateEnum.Idle);
        if(isRespawn)
        {
            Vector2Int roomPos = CameraMover.RoomPosForWorldPos(transform.position);
            transform.position = respawnPosition.Value;
            GridRegistry.Instance.ReorderRoomGridObject(this, roomPos);
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

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == GameConstants.PLAYER_TAG)
        {
            isOnGrandmaVar.Value = true;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other) 
    {
        if(other.tag == GameConstants.PLAYER_TAG)
        {
            isOnGrandmaVar.Value = false;
        }
    }

    public override bool GrabCharacter(CharacterMovement character)
    {
        physicalCollider.enabled = false;
        SetState(GrandmaStateEnum.Idle);
        character.transform.parent = representation.transform;
        character.transform.position = throwAnchor.position;

        if(grabStrings != null)
        {
            balloon.ShowText(grabStrings.GetRandomSelection());
        }
        
        return true;
    }

    public override void ReleaseCharacter(CharacterMovement character, bool throwChar)
    {
        character.transform.parent = null;
        character.transform.right = Vector2.right;
        if(throwChar)
        {
            soundHelper.Value.PlaySound(GameSoundTag.SFX_THROW);
            character.JumpTo((Vector3)targetPosVar.Value + character.transform.position.z * Vector3.up);
            if(throwStrings != null)
            {
                balloon.ShowText(throwStrings.GetRandomSelection());
            }
        }
        else
        {
            character.transform.position  = GetEmptyAdjacentPos();

            if(cancelThrowStrings != null)
            {
                balloon.ShowText(cancelThrowStrings.GetRandomSelection());
            }

        }
        physicalCollider.enabled = true;
    }

    protected Vector2Int[] GetUnoccupiedAdjacent(bool ignoreDiagonals = false)
    {
        Predicate<GridEntity> ignoreself = (GridEntity entity) 
        => { return entity.transform == transform; };

        return roomController.GetUnoccupiedNeighbours((Vector2Int)this.GridPos, ignoreself, ignoreDiagonals);
    }

    private Vector3 GetEmptyAdjacentPos()
    {
        Vector2 cellSize = CameraMover.Instance.CellSize;

        Predicate<GridEntity> ignoreself = (GridEntity entity) 
                => { return entity.transform == transform; };

        Vector2Int[] positions = GetUnoccupiedAdjacent();
        List<Vector2Int> availPos = new List<Vector2Int>();
        List<Vector2Int> cracks = new List<Vector2Int>();

        foreach(Vector2Int gridpos in positions)
        {
            GridEntity[] entities =GridRegistry.Instance.GetEntitiesAtPos((Vector3Int)gridpos);
            bool isValid = true;
            foreach(GridEntity entity in entities)
            {
                Crack crack = entity.GetComponent<Crack>();
                if(crack != null)
                {
                    isValid = false;
                    if(!crack.IsHole)
                    {
                        cracks.Add(gridpos);
                    }
                    break;
                }
            }

            if(isValid)
            {
                availPos.Add(gridpos);
            }
        }

        Vector3 pos = RepresentationPos;
        if(availPos.Count > 0)
        {
            int index = UnityEngine.Random.Range(0,availPos.Count);
            pos = CameraMover.WorldPosForGridPos((Vector3Int)availPos[index], 0);
        }
        else if(cracks.Count > 0)
        {
            int index = UnityEngine.Random.Range(0,cracks.Count);
            pos = CameraMover.WorldPosForGridPos((Vector3Int)cracks[index], 0);
        }

        Vector3 dir = (pos - RepresentationPos);
        return RepresentationPos + dir * Mathf.Min(maxDropDistance, Vector2.Distance(pos, RepresentationPos));
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
        
        Vector2Int[] path = grannyPath.PerformSearch(pos, goal, grandmaPathFeeder);

        if(ignoreSelf)
        {
            roomController.RemoveEntityToIgnore(transform);
        }

        return path;
    }

    public IEnumerator SimpleWalkRoutine(Vector2Int[] path, float speed, Action callback)
    {
        List<Vector3> worldPath = new List<Vector3>();
        
        foreach(Vector2Int pos in path)
        {
            Vector3 worldPos = CameraMover.WorldPosForGridPos((Vector3Int)pos, 0);
            worldPath.Add(worldPos);
        }
        yield return SimpleWalkRoutine(worldPath.ToArray(), speed, callback, transform);
    }
    
    public IEnumerator SimpleWalkRoutine(Vector3[] path, float speed, Action callback)
    {
        yield return SimpleWalkRoutine(path, speed, callback, transform);
    }

    public IEnumerator SimpleWalkRoutine(Vector3[] path, float speed, Action callback, Transform transformToMove)
    {   
        if(path == null || path.Length == 0)
        {
            callback?.Invoke();
            yield break;
        }

        foreach(Vector3 worldPos in path)
        {
            Vector2 dir = (worldPos - transformToMove.position).normalized;
            float dist = Vector2.Distance(worldPos, transformToMove.position);
            
            while(Time.deltaTime * speed <= dist)
            {
                yield return new WaitForEndOfFrame();
                float delta = speed * Time.deltaTime;
                transformToMove.position += (Vector3)dir * delta;
                dist -= delta;
            }
            transformToMove.position = worldPos;
        }
        callback?.Invoke();
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
        Balloon.HideBalloon(true);
        ResetGrandma(false);
        currentRoom = CameraMover.RoomPosForGridPos(GridPos);
    }

    public override void OnRoomLeave() 
    {
        Balloon.HideBalloon(true);
        Vector2Int newRoom = CameraMover.RoomPosForGridPos(characterVar.Value.GridPos);
        if(currentRoom == newRoom)
        {
            return;
        }

        this.transform.position = CameraMover.WorldPosForGridPos(characterVar.Value.GridPos, 0);
        this.originalPosition = transform.position;
        GridRegistry.Instance.ReorderRoomGridObject(this, currentRoom);
    }

    public override bool CheckLeaveRoom(Vector3 goalPos, Action callback)
    {
        Vector3Int gridGoalPos = CameraMover.GridPosForWorldPos(goalPos);
        Vector3 clampedworldPos = CameraMover.WorldPosForGridPos(gridGoalPos, 0);
        
        roomController.GenerateBorderNeighbours = true;
        Vector2Int[] path = GetPath(clampedworldPos, true);
        roomController.GenerateBorderNeighbours = false;

        if(gridGoalPos == GridPos || path != null && path.Length > 0)
        {
            SetMoveTarget(clampedworldPos);
            if(successLeaveStrings != null)
            {
                balloon.ShowText(successLeaveStrings.GetRandomSelection());
            }
            
            onNewRoomCallback = callback;
            SetState(GrandmaStateEnum.MovingToNext);
            return true;
        }
        else
        {
            if(balloon.IsLeavingOrHidden && failLeaveStrings != null)
            {
                balloon.ShowText(failLeaveStrings.GetRandomSelection());
            }
        }
        return false;
    }
    
    public void OnReachedNewRoom(Vector2Int oldRoomPos)
    {
        this.originalPosition = transform.position;
        onNewRoomCallback?.Invoke();
        onNewRoomCallback = null;   
        GridRegistry.Instance.ReorderRoomGridObject(this, oldRoomPos);
    }

    public override void OnBacktracking()
    {
        if(balloon.IsLeavingOrHidden && backtrackStrings != null)
        {
            balloon.ShowText(backtrackStrings.GetRandomSelection());
        }
    }
}
