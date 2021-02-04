using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapUtils
{
    public static T[] GetTiles<T>(this Tilemap tilemap) where T : TileBase
    {
        List<T> tiles = new List<T>();
        
        for (int y = tilemap.origin.y; y < (tilemap.origin.y + tilemap.size.y); y++)
        {
            for (int x = tilemap.origin.x; x < (tilemap.origin.x + tilemap.size.x); x++)
            {
                T tile = tilemap.GetTile<T>(new Vector3Int(x, y, 0));
                if (tile != null)
                {
                    tiles.Add(tile);
                }
            }
        }
        return tiles.ToArray();
    }

    public static Vector3Int[] GetTileIndexes(this Tilemap tilemap)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        

        for (int y = tilemap.origin.y; y < (tilemap.origin.y + tilemap.size.y); y++)
        {
            for (int x = tilemap.origin.x; x < (tilemap.origin.x + tilemap.size.x); x++)
            {
                for (int z = tilemap.origin.z; z < (tilemap.origin.z + tilemap.size.z); z++)
                {
                    Vector3Int pos = new Vector3Int(x, y, z);
                    
                    TileBase tile = tilemap.GetTile<TileBase>(pos);
                    if (tile != null)
                    {
                        positions.Add(pos);
                    }
                }
            }
        }
        return positions.ToArray();
    }

    public static bool IsPosBelow(Vector3Int pos, Vector3Int posToCheck)
    {
        return pos.x == posToCheck.x && pos.y == posToCheck.y && posToCheck.z < pos.z;
    }

    public static bool IsPosAbove(Vector3Int pos, Vector3Int posToCheck)
    {
        return pos.x == posToCheck.x && pos.y == posToCheck.y && posToCheck.z > pos.z;
    }
}