using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;
using  UnityEngine.Tilemaps;

public class TargetMover : MonoBehaviour
{
    [SerializeField]
    private IInputController input;
    [SerializeField]
    private CharacterStateVar stateVar;

    [SerializeField]
    private float moveSpeed = 5.0f;

    [SerializeField]
    private Vector2Var targetPosVar;

    [SerializeField]
    private BoolVar validThrow;

    [SerializeField]
    private GrandmaScriptVar grandmaVar;

    [SerializeField]
    private RoomTileController controller;

    [SerializeField]
    private TileBase[] validTiles;
    private HashSet<System.Type> validTypes = new HashSet<System.Type>();

    [SerializeField]
    private float range = 4;

    private void Start() 
    {
        foreach(TileBase tile in validTiles)
        {
            validTypes.Add(tile.GetType());
        }

        stateVar.OnChange += OnStateChange;
        OnStateChange(CharacterState.Idle, stateVar.Value);
        validThrow.Value = false;
    }

    private void OnDestroy() 
    {
        stateVar.OnChange -= OnStateChange;
    }

    private void OnStateChange(CharacterState oldVal, CharacterState newVal)
    {
        gameObject.SetActive(newVal == CharacterState.Throwing);
        if(!gameObject.activeInHierarchy)
        {
            return;
        }
        
        Vector2 resetPos = (Vector2)grandmaVar.Value.transform.position;
        transform.position = (Vector3) resetPos + Vector3.up * transform.position.z;
        targetPosVar.Value = transform.position;
    }

    private void Update()
    {
        Vector3 dir = input.GetMovementAxis();
        if(!Mathf.Approximately(dir.magnitude, 0))
        {
            Vector3 goalPos = transform.position + moveSpeed*dir * Time.deltaTime;
            if(Vector2.Distance(goalPos,grandmaVar.Value.RepresentationPos) > range)
            {
                Vector2 offsetDir = ((Vector2)goalPos - (Vector2)grandmaVar.Value.RepresentationPos).normalized;
                goalPos = grandmaVar.Value.RepresentationPos + (Vector3)offsetDir * range;
            }
            goalPos = controller.ClampWorldPosToRoom(goalPos);
            transform.position = goalPos;
            targetPosVar.Value  = transform.position - CameraMover.Instance.CellSize/2.0f;
        }

        validThrow.Value = IsPositionValidThrow();
    }


    private bool IsPositionValidThrow()
    {
        Vector3Int gridPos = CameraMover.GridPosForWorldPos(transform.position);
        if(!controller.IsEmptyPos((Vector2Int)gridPos))
        {
            return false;
        }
        
        Vector2 targetSize = CameraMover.Instance.CellSize * 0.15f;
        for(float x = -0.5f; x <= 0.5f; x++)
        {
            for(float y = -0.5f; y <= 0.5f; y++)
            {
                Vector3 checkWorldPos = new Vector3(transform.position.x + x*targetSize.x,
                                               transform.position.y + y*targetSize.y,
                                               transform.position.z);
                Vector3Int checkPos = CameraMover.GridPosForWorldPos(checkWorldPos);
                
                if(!controller.IsEmptyPos((Vector2Int)checkPos))
                {
                    return false;
                }
            }
        }

        TileBase tile = controller.GetTileForWorldPos(transform.position);
        
        return tile != null && validTypes.Contains(tile.GetType());
    }
}
