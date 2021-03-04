using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCollection : ScriptableObject
{
    [SerializeField]
    private Vector2Int[] rooms;

    public Vector2Int[] Rooms => rooms;
    public int Length => rooms.Length;

    private Dictionary<Vector2Int, int> indexesDict = new Dictionary<Vector2Int, int>();
    private void OnEnable() 
    {
        indexesDict.Clear();
        for(int i = 0; i < rooms.Length; i++)
        {
            Vector2Int room = rooms[i];
            if(indexesDict.ContainsKey(room))
            {
                Debug.LogError("Collision of room keys at " + room);
            }
            indexesDict[room] = i;
        }
    }

    public Vector2Int GetRoom(int index)
    {
        if(rooms.Length <= 0)
        {
            return Vector2Int.zero;
        }
        index = Mathf.Clamp(index, 0, rooms.Length-1);
        return rooms[index];
    }

    public int GetIndexOfRoom(Vector2Int room)
    {
        if(indexesDict.ContainsKey(room))
        {
            return indexesDict[room];
        }
        return -1;
    }

    public bool ContainsRoom(Vector2Int room)
    {
        return indexesDict.ContainsKey(room);
    }

}
