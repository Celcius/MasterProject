using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridUtils
{
    public static Vector3 WorldPosClampedToGridPos(Vector3 worldPos, Vector2 screenSize)
    {
        Vector3Int Vector3Int = GridPosForWorldPos(worldPos, screenSize);
        return WorldPosForGridPos(Vector3Int, worldPos.z, screenSize);
    }

    public static Vector3 WorldPosForGridPos(Vector3Int pos, float posZ, Vector2 screenSize)
    {
        return new Vector3(pos.x *screenSize.x, pos.y * screenSize.y,
                                posZ); 
    }

    public static Vector3Int GridPosForWorldPos(Vector3 pos, Vector2 screenSize)
    {

        return new Vector3Int((int)(pos.x/screenSize.x), (int)(pos.y/screenSize.y), (int)pos.z);
    }

    
    public static bool ArePosAdjacent(Vector3Int pos, Vector3Int otherPos)
    {
        bool xDistance = Mathf.Abs(pos.x - otherPos.x) <=1.0f;
        bool yDistance = Mathf.Abs(pos.y - otherPos.y) <= 1.0f;
        return xDistance && yDistance;
    }

    public static Vector2Int RoomPosForGridPos(Vector3Int pos, Vector2 screenSize)
    {
        return RoomPosForWorldPos(WorldPosForGridPos(pos, 0, screenSize), screenSize);
    }

    public static Vector2Int RoomPosForWorldPos(Vector3 pos, Vector2  screenSize)
    {      
        Vector2 CamPos = new Vector2(((pos.x+screenSize.x/2.0f)/screenSize.x),
                                    ((pos.y+screenSize.y/2.0f)/screenSize.y));
        CamPos.x = (int)(CamPos.x < 0? CamPos.x-1 : CamPos.x); 
        CamPos.y = (int)(CamPos.y < 0? CamPos.y-1 : CamPos.y);
        return new Vector2Int((int)CamPos.x, (int)CamPos.y);
    }

    public static Vector3 WorldPosForRoomPos(Vector2Int roomPos, Vector2 screenSize, float posZ)
    {
        return new Vector3(roomPos.x * screenSize.x, roomPos.y * screenSize.y, posZ);
    }
}
