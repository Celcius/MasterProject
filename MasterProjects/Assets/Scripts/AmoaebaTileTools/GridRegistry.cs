using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridRegistry : Singleton<GridRegistry>
{
    private Dictionary<Vector2Int, List<GridEntity>> gridObjects = new Dictionary<Vector2Int, List<GridEntity>>();
    private List<GridEntity> allObjects = new List<GridEntity>();
    public List<GridEntity> AllGridObjects => allObjects;

    public List<T> GetRoomObjects<T>(Vector2Int roomVector3Int) where T : GridEntity
    {
        if(!gridObjects.ContainsKey(roomVector3Int))
        {
            return new List<T>();            
        }
        
        List<GridEntity> objects = gridObjects[roomVector3Int];
        List<T> ret = new List<T>();

        foreach(GridEntity obj in objects)
        {
            if(obj.GetType().IsSubclassOf(typeof(T)) || obj.GetType() == typeof(T))
            {
                ret.Add((T) obj);
            }
        }
        return ret;
    }

    public void AddRoomGridObject(GridEntity entity)
    {
        Vector2Int roomPos = entity.RoomGridPos;
        if(!gridObjects.ContainsKey(roomPos))
        {
            gridObjects[roomPos] = new List<GridEntity>();
        }
        gridObjects[roomPos].Add(entity);
        allObjects.Add(entity);
    }

    public GridEntity GetEntityAtPos(Vector3Int gridPos)
    {
        Vector2Int roomPos = CameraMover.RoomPosForGridPos(gridPos);
        if(!gridObjects.ContainsKey(roomPos))
        {
            return null;
        }

        List<GridEntity> entities = gridObjects[roomPos];
        foreach(GridEntity entity in entities)
        {
            if(entity.Occupies(gridPos))
            {
                return entity;
            }
        }

        return null;
    }

    public void RemoveRoomGridObject(GridEntity entity)
    {
        RemoveGridObject(entity, entity.RoomGridPos);
    }

    public void RemoveGridObject(GridEntity behaviour, Vector2Int oldRoomPos)
    {
        if(gridObjects.ContainsKey(oldRoomPos))
        {
            gridObjects[oldRoomPos].Remove(behaviour);
        }
        allObjects.Remove(behaviour);
    }

    public void ReorderRoomGridObject(GridEntity behaviour, Vector2Int oldRoomPos)
    {
        RemoveGridObject(behaviour, oldRoomPos);
        AddRoomGridObject(behaviour);
    }
}
