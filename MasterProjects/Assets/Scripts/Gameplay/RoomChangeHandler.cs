using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class RoomChangeHandler : SerializedMonoBehaviour
{
    [SerializeField]
    private RoomHandler roomHandler;

    protected virtual void Start() 
    {
        roomHandler.onEnter += OnRoomEnter;
        roomHandler.onLeave += OnRoomLeave;
    }

    protected virtual void OnDestroy() 
    {
        roomHandler.onEnter -= OnRoomEnter;
        roomHandler.onLeave -= OnRoomLeave;
    }

    public abstract void OnRoomEnter(Vector2Int pos);

    public abstract void OnRoomLeave(Vector2Int pos);
}
