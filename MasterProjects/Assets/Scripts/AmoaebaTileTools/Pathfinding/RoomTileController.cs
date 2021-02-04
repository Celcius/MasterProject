﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

namespace AmoaebaUtils
{
public class RoomTileController : ScriptableObject, AStarMapFeeder<Vector2Int>
{
    [SerializeField]
    private CameraMoverVar CamMoverVar;

    private Vector2Int roomGridPos;
    private BoundsInt roomGridPosBounds;

    [SerializeField]
    private TilemapVar referenceMap;

    [SerializeField]
    private bool isReferenceMapWalls = true;

    private HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

    private void OnEnable()
    {
        CamMoverVar.OnChange += OnCameraChanged;
        if(CamMoverVar.Value != null)
        {
            OnNewRoom(Vector2Int.zero, CamMoverVar.Value.CurrentRoomPos);
        }
    }

    private void OnDisable() 
    {
        CamMoverVar.OnChange -= OnCameraChanged;    
    }

    private void OnCameraChanged(CameraMover oldCam, CameraMover newCam)
    {
        if(oldCam != null)
        {
            oldCam.OnCameraMoveEnd -= OnNewRoom;
        }
        
        if(newCam != null)
        {
            newCam.OnCameraMoveEnd += OnNewRoom;
        }
        OnNewRoom(Vector2Int.zero, newCam.CurrentRoomPos);
    }

    private void OnNewRoom(Vector2Int oldRoomGridPos, Vector2Int newRoomGridPos)
    {
        UpdateMapPositions(newRoomGridPos);
    }

    private void UpdateMapPositions(Vector2Int roomGridPos)
    {
        this.roomGridPos = roomGridPos;
        Vector3Int roomSize = new Vector3Int(Mathf.RoundToInt(CamMoverVar.Value.RoomSize.x), 
                                             Mathf.RoundToInt(CamMoverVar.Value.RoomSize.y),
                                             1);
        Vector3Int pos = new Vector3Int(roomSize.x * roomGridPos.x, roomSize.y * roomGridPos.y, 0);
        this.roomGridPosBounds = new BoundsInt(pos, roomSize);
        
        floorPositions.Clear();

        if(referenceMap.Value == null)
        {
            return;
        }

        for(int x = roomGridPosBounds.xMin; x <= roomGridPosBounds.xMax; x++)
        {
            for(int y = roomGridPosBounds.yMin; y <= roomGridPosBounds.yMax; y++)
            {
                Vector2Int attemptPos = new Vector2Int(x,y);
                Vector3 worldPos = CameraMover.WorldPosForGridPos((Vector3Int)attemptPos, 0);

                bool hasTile = GridUtils.GetTileForWorldPos(referenceMap.Value, worldPos) != null;

                if(isReferenceMapWalls != hasTile)
                {
                    floorPositions.Add(attemptPos);
                }
            }
        }
    }

    public bool IsInCurrentRoom(Vector2Int gridPos)
    {
        bool containsPos = roomGridPosBounds.Contains((Vector3Int)gridPos);
        return containsPos;
    }

    public bool IsFloorPos(Vector2Int gridPos)
    {
        return floorPositions.Contains(gridPos); 
    }

    public bool IsEmptyPos(Vector2Int gridPos)
    {
        return IsFloorPos(gridPos) && GridRegistry.Instance.GetEntityAtPos((Vector3Int)gridPos) == null;
    }

    
    public Vector2Int[] GetNeighbours(Vector2Int pos)
    {
        List<Vector2Int> retPos = new List<Vector2Int>();
        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                if(Mathf.Abs(x) == Mathf.Abs(y))
                {
                    continue;
                }
                Vector2Int attemptPos = new Vector2Int(pos.x + x, pos.y + y);
                if(IsInCurrentRoom(attemptPos) && IsEmptyPos(attemptPos))
                {
                    retPos.Add(attemptPos);
                }
            }
        }
        return retPos.ToArray();
    }

    public float GetMoveCost(Vector2Int origin, Vector2Int dest)
    {
        float offset = Mathf.Abs(dest.x - origin.x) 
                       + Mathf.Abs(dest.y - origin.y);
        return (offset <= 1)? offset : float.MaxValue;
    }
    
    public float GetDistanceEstimation(Vector2Int origin, Vector2Int dest)
    {
        return Vector2Int.Distance(origin, dest);
    }

    public bool SameNode(Vector2Int node1, Vector2Int node2)
    {
        return node1 == node2;
    }
}
}