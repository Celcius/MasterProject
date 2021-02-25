using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraMover))]
public class RoomHandler : MonoBehaviour
{
    private CameraMover mover;

    public delegate void OnRoomChange(Vector2Int newRoom);
    public event OnRoomChange onEnter;
    public event OnRoomChange onLeave;

    private Vector2Int currentRoom;
    public Vector2Int CurrentRoom => currentRoom;

    
    protected virtual void Start()
    {
        mover = GetComponent<CameraMover>();
        mover.OnCameraMoveStart += HandleCameraMoveStart;
        mover.OnCameraMoveEnd += HandleCameraMoveEnd;
        //ServiceLocator.Instance.respawn += RespawnRoom;
        DisableAll();
        EnableCurrentRoom();
    }
    protected virtual void DisableAll()
    {
        List<GridEntity> gridElements =  new List<GridEntity>(GridRegistry.Instance.AllGridObjects);
        if(gridElements != null)
        {
            foreach(GridEntity behaviour in gridElements)
            {
                behaviour.OnRoomLeave();
            }
        }
    }
    
    protected virtual void EnableCurrentRoom()
    {
        Vector2Int room = CameraMover.RoomPosForWorldPos(mover.LookAtGridEntity.transform.position);
        OnRoomEnter(true, room);
    }

    protected virtual void HandleCameraMoveStart(Vector2Int roomPos, Vector2Int newRoomPos)
    {
        List<GridEntity> gridElements =  GridRegistry.Instance.GetRoomObjects<GridEntity>(roomPos);
        
        if(gridElements != null)
        {
            foreach(GridEntity behaviour in gridElements)
            {
                behaviour.OnRoomWillLeave();
            }
        }

        gridElements =  GridRegistry.Instance.GetRoomObjects<GridEntity>(newRoomPos);
        if(gridElements != null)
        {
            foreach(GridEntity behaviour in gridElements)
            {
                behaviour.OnRoomWillEnter();
            }
        }
    }

    public virtual void RespawnRoom()
    {
        OnRoomEnter(false, mover.CurrentRoomPos);
        DisableAll();
        EnableCurrentRoom();
    }

    protected virtual void HandleCameraMoveEnd(Vector2Int startPos, Vector2Int endPos)
    {
        OnRoomEnter(false, startPos);
        OnRoomEnter(true, endPos);
        mover.LookAtGridEntity.OnRoomWillEnter();
        mover.LookAtGridEntity.OnRoomEnter();
    }

    protected virtual void OnRoomEnter(bool isEntering, Vector2Int roomPos)
    {
        if(isEntering)
        {
            currentRoom = roomPos;
        }
        
        List<GridEntity> gridElements =  GridRegistry.Instance.GetRoomObjects<GridEntity>(roomPos);
        BroadCastEnter(isEntering, gridElements);
        BroadCastEnter(isEntering, GridRegistry.Instance.NonRoomObjects);

        if(isEntering)
        {
            onEnter?.Invoke(roomPos);
        }
        else
        {
            onLeave?.Invoke(roomPos);
        }
    }

    private void BroadCastEnter(bool isEntering, List<GridEntity> gridElements)
    {
        if(gridElements != null)
        {
            foreach(GridEntity behaviour in gridElements)
            {

                if(isEntering)
                {
                    behaviour.OnRoomWillEnter();
                    behaviour.OnRoomEnter();
                }
                else
                {
                    behaviour.OnRoomWillLeave();
                    behaviour.OnRoomLeave();
                }
            }
        }
    }
}
