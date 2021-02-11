using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using AmoaebaUtils;

namespace AmoaebaUtils
{
public class RoomTileController : ScriptableObject, AStarMapFeeder<Vector2Int>
{
    [SerializeField]
    private CameraMoverVar CamMoverVar;

    private Vector2Int roomGridPos;
    private BoundsInt roomGridPosBounds;
    private Bounds roomBounds;

    [SerializeField]
    private TilemapVar referenceMap;

    [SerializeField]
    private bool isReferenceMapWalls = true;

    [SerializeField]
    private TileBase[] tilesToIgnore;
    private HashSet<System.Type> tilesToIgnoreHash = new HashSet<System.Type>();

    private HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

    private List<Transform> entitiesToIgnore = new List<Transform>();

    public bool IgnoreOtherRoom {get; set;}

    private void OnEnable()
    {
        IgnoreOtherRoom = true;
        entitiesToIgnore.Clear();
        tilesToIgnoreHash.Clear();
        foreach(TileBase tileBase in tilesToIgnore)
        {
            tilesToIgnoreHash.Add(tileBase.GetType());
        }

        CamMoverVar.OnChange += OnCameraChanged;
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
        
        Vector3 cellSize = CamMoverVar.Value.CellSize;
        cellSize.z = 1.0f;

        Vector3 size = Vector3.Scale(((Vector3)roomSize), cellSize);
        Vector3 boundsPos = Vector3.Scale(((Vector3)pos), cellSize) + size /2.0f;
        roomBounds = new Bounds(boundsPos, size);
        
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

                TileBase tile = GridUtils.GetTileForWorldPos(referenceMap.Value, worldPos);
                
                bool hasTile = (tile != null && !tilesToIgnoreHash.Contains(tile.GetType()));

                if(isReferenceMapWalls != hasTile)
                {
                    floorPositions.Add(attemptPos);
                }
            }
        }
    }

    public TileBase GetTileForWorldPos(Vector3 worldPos)
    {
        return GridUtils.GetTileForWorldPos(referenceMap.Value, worldPos);
    }

    public bool IsInCurrentRoom(Vector2Int gridPos)
    {
        bool containsPos = roomGridPosBounds.Contains((Vector3Int)gridPos);
        return containsPos;
    }

    public Vector3Int FindEmptyPosInDir(Vector3Int searchPos, Vector2Int dir)
    {
        Vector3Int searchBelow = searchPos;
        Tilemap map = referenceMap.Value;
        while(roomGridPosBounds.Contains(searchBelow))
        {
            searchBelow += (Vector3Int)dir;
            Vector3 worldPos = CameraMover.WorldPosForGridPos(searchBelow, 0);
            TileBase tile = GetTileForWorldPos(worldPos);
            if(tile == null)
            {
                return searchBelow;
            }
        }
        return searchPos;
    }

    public Vector3 ClampWorldPosToRoom(Vector3 worldPos, bool clampToGridBounds = true)
    {
        Vector3 min = roomBounds.min + (clampToGridBounds? CamMoverVar.Value.CellSize/2.0f : Vector3.zero);
        Vector3 max = roomBounds.max - (clampToGridBounds? CamMoverVar.Value.CellSize/2.0f : Vector3.zero);
        Vector3 clampedPos = new Vector3(Mathf.Clamp(worldPos.x, min.x, max.x),
                                         Mathf.Clamp(worldPos.y, min.y, max.y),
                                        worldPos.z);
        return clampedPos;
    }

    public bool IsFloorPos(Vector2Int gridPos)
    {
        return floorPositions.Contains(gridPos); 
    }

    public bool IsEmptyPos(Vector2Int gridPos, Transform[] toIgnore = null)
    {
        if(!IsFloorPos(gridPos) && (IsInCurrentRoom(gridPos) || IgnoreOtherRoom))
        {
            return false;
        }
        
        
        GridEntity[] foundEntities = GridRegistry.Instance.GetEntitiesAtPos((Vector3Int)gridPos);
        if(foundEntities == null || foundEntities.Length == 0)
        {
            return true;
        }
        
        foreach(GridEntity entity in foundEntities)
        {
            if(IsBlockingEntity(entity, toIgnore))
            {
                return false;
            }
        }

        return true;
    }

    public bool IsStandablePos(Vector2Int gridPos, Transform[] toIgnore = null)
    {
        bool isEmpty = IsEmptyPos(gridPos, toIgnore);
        if(!isEmpty)
        {
            return false;
        }

        GridEntity[] foundEntities = GridRegistry.Instance.GetEntitiesAtPos((Vector3Int)gridPos);
        foreach(GridEntity entity in foundEntities)
        {
            if(!entity.AllowsStand)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsBlockingEntity(GridEntity foundEntity, Transform[] toIgnore)
    {
        if(foundEntity == null || !foundEntity.IsBlocking)
        {
            return false;
        }

        if(entitiesToIgnore != null && entitiesToIgnore.Count > 0)
        {
            foreach(Transform entity in entitiesToIgnore)
            {
                if(foundEntity.transform == entity)
                {
                    return false;
                }
            }
        }
        
        if(toIgnore != null && toIgnore.Length > 0)
        {
            foreach(Transform entity in toIgnore)
            {
                if(foundEntity.transform == entity)
                {
                    return false;
                }
            }
        }
        
        return true;
    }

    public Vector2Int[] GetUnoccupiedNeighbours(Vector2Int pos)
    {
        Vector2Int[] neighbours = GetNeighbours(pos);
        List<Vector2Int> ret = new List<Vector2Int>();
        foreach(Vector2Int neighbour in neighbours)
        {
            if(IsEmptyPos(neighbour))
            {
                ret.Add(neighbour);
            }
        }
        return ret.ToArray();
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
                if(IsEmptyPos(attemptPos))
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

    public void AddEntityToIgnore(Transform entity)
    {
        entitiesToIgnore.Add(entity);
    }

    public void RemoveEntityToIgnore(Transform entity)
    {
        entitiesToIgnore.Remove(entity);
    }
}
}