using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;

public class TilemapDivisionTool : MonoBehaviour
{
    [SerializeField]
    private CameraMover cameraMover;

    [SerializeField]
    private Tilemap emptyTilemap;

    [Button]
    public void DivideMap()
    {
        Tilemap[] divisionMapTargets = GetComponentsInChildren<Tilemap>();
        if(divisionMapTargets == null || divisionMapTargets.Length == 0)
        {
            return;
        }

        for(int i = divisionMapTargets.Length-1; i >= 0; i--)
        {
            DivideMap(divisionMapTargets[i], true);
        }
    }    

    [Button]
    public void CollapseMaps()
    {
        CollapseMaps(true);
    }

    public Tilemap CollapseMaps(bool destroyMaps)
    {
        Tilemap[] maps = GetComponentsInChildren<Tilemap>();
        if(maps == null || maps.Length == 0)
        {
            return null;
        }

        if(maps.Length == 1)
        {
            return maps[0];
        }

        Tilemap finalMap = CreateNewMap("Final Map");

        System.Action<Tilemap, Vector3Int> iterateCallback = (Tilemap itMap, Vector3Int gridPos) =>
        {
            TileBase tile = itMap.GetTile(gridPos);
            finalMap.SetTile(gridPos, tile);
            finalMap.SetTileFlags(gridPos, itMap.GetTileFlags(gridPos));
        };

        for(int i = maps.Length-1; i >= 0; i--)
        {
            IterateMap(maps[i], iterateCallback);
            if(destroyMaps)
            {
                DestroyMap(maps[i]);
            }
        }
        return finalMap;
    }

    public void DivideMap(Tilemap map, bool destroyMap) 
    {
        Dictionary<Vector2Int, Tilemap> roomTilemaps = new Dictionary<Vector2Int, Tilemap>();
        
        System.Action<Tilemap, Vector3Int> iterateCallback = (Tilemap itMap, Vector3Int gridPos) =>
        {
                Vector2Int roomPos = GridUtils.RoomPosForGridPos(gridPos,
                                                                 cameraMover.CellSize, 
                                                                 cameraMover.RoomSize);
                TileBase tile = itMap.GetTile(gridPos);
                if(tile == null)
                {
                    return;
                }

                Tilemap mapToFill;
                if(roomTilemaps.ContainsKey(roomPos))
                {
                    mapToFill = roomTilemaps[roomPos];
                }
                else
                {
                    mapToFill = CreateNewMap("Map" + roomPos);
                    roomTilemaps.Add(roomPos, mapToFill);
                }

                mapToFill.SetTile(gridPos, tile);
                mapToFill.SetTileFlags(gridPos, itMap.GetTileFlags(gridPos));
        };

        IterateMap(map, iterateCallback);
        if(destroyMap)
        {
            DestroyMap(map);
        }
    }

    private void IterateMap(Tilemap map, System.Action<Tilemap, Vector3Int> callback)
    {
        BoundsInt mapBounds = map.cellBounds;
        for(int x = mapBounds.xMin; x <= mapBounds.xMax; x++)
        {
            for(int y = mapBounds.yMin; y <= mapBounds.yMax; y++)
            {
                Vector3Int gridPos = new Vector3Int(x,y,0);
                callback?.Invoke(map, gridPos);
            }
        }
    }

    private Tilemap CreateNewMap(string name)
    {
        Tilemap map = Instantiate<Tilemap>(emptyTilemap, transform);
        map.transform.localPosition = Vector3.zero;
        map.gameObject.name = name;
        return map;
    }

    private void DestroyMap(Tilemap map)
    {
        if(Application.isPlaying)
        {
            Destroy(map.gameObject);
        }
        else
        {
            DestroyImmediate(map.gameObject);
        }
    }


}
