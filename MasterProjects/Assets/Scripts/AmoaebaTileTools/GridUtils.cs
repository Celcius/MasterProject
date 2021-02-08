using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class GridUtils
{
    public static Vector3 WorldPosRoundedToGrid(Vector3 worldPos, Vector2 gridUnitSize)
    {
        Vector3Int Vector3Int = GridPosForWorldPos(worldPos, gridUnitSize);
        return WorldPosForGridPos(Vector3Int, worldPos.z, gridUnitSize);
    }

    public static Vector3 WorldPosForGridPos(Vector3Int pos, float posZ, Vector2 gridUnitSize)
    {
        return new Vector3(pos.x *gridUnitSize.x, pos.y * gridUnitSize.y,
                                posZ); 
    }

    public static Vector3Int GridPosForWorldPos(Vector3 pos, Vector2 gridUnitSize)
    {
        return new Vector3Int((int)(pos.x/gridUnitSize.x), 
                              (int)(pos.y/gridUnitSize.y), 
                              Mathf.RoundToInt(pos.z));
    }

    
    public static bool ArePosAdjacent(Vector3Int pos, Vector3Int otherPos)
    {
        bool xDistance = Mathf.Abs(pos.x - otherPos.x) <=1.0f;
        bool yDistance = Mathf.Abs(pos.y - otherPos.y) <= 1.0f;
        return xDistance && yDistance;
    }

    public static Vector2Int RoomPosForGridPos(Vector3Int pos, Vector2 gridSize, Vector2 roomSize)
    {
        return RoomPosForWorldPos(WorldPosForGridPos(pos, 0, gridSize), roomSize, gridSize);
    }

    public static Vector2Int RoomPosForWorldPos(Vector3 pos, Vector2  roomSize, Vector2 gridUnitSize)
    {      
        Vector2 CamPos = new Vector2((pos.x/gridUnitSize.x/roomSize.x),
                                    (pos.y/gridUnitSize.x/roomSize.y));
        CamPos.x = (int)(CamPos.x < 0? CamPos.x-1 : CamPos.x); 
        CamPos.y = (int)(CamPos.y < 0? CamPos.y-1 : CamPos.y);
        return new Vector2Int((int)CamPos.x, (int)CamPos.y);
    }

    public static Vector3 WorldPosForRoomPos(Vector2Int roomPos, Vector2 roomSize, Vector2 gridUnitSize, float posZ)
    {
        return new Vector3(roomPos.x * roomSize.x* gridUnitSize.x + roomSize.x* gridUnitSize.x/2.0f, 
                           roomPos.y * roomSize.y* gridUnitSize.x + roomSize.y* gridUnitSize.x/2.0f, 
                           posZ);
    }

    public static TileBase GetTileForWorldPos(Tilemap map, Vector3 worldPos)
    {
        
        Vector3Int tilemapPos = map.WorldToCell(worldPos);
        return map.GetTile(tilemapPos);
    }
}
