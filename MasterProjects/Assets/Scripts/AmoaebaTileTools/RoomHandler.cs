using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraMover))]
public class RoomHandler : MonoBehaviour
{
    private CameraMover mover;
    
    private void Start()
    {
        mover = GetComponent<CameraMover>();
        mover.OnCameraMoveStart += HandleCameraMoveStart;
        mover.OnCameraMoveEnd += HandleCameraMoveEnd;
        //ServiceLocator.Instance.respawn += RespawnRoom;
        DisableAll();
        EnableCurrentRoom();
    }
    private void DisableAll()
    {
        List<GridEntity> gridElements =  GridRegistry.Instance.AllGridObjects;
        if(gridElements != null)
        {
            foreach(GridEntity behaviour in gridElements)
            {
                behaviour.OnRoomLeave();
            }
        }
    }
    private void EnableCurrentRoom()
    {
        Vector2Int room = CameraMover.RoomPosForWorldPos(mover.LookAtGridEntity.transform.position);
        OnRoomEnter(true, room);
    }

    private void HandleCameraMoveStart(Vector2Int roomPos, Vector2Int newRoomPos)
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

    private void RespawnRoom()
    {
        Vector2Int pos = CameraMover.RoomPosForWorldPos(mover.LookAtGridEntity.transform.position);
        OnRoomEnter(false, pos);
        EnableCurrentRoom();
    }

    private void HandleCameraMoveEnd(Vector2Int startPos, Vector2Int endPos)
    {
        OnRoomEnter(false, startPos);
        OnRoomEnter(true, endPos);
        mover.LookAtGridEntity.OnRoomWillEnter();
        mover.LookAtGridEntity.OnRoomEnter();
    }

    private void OnRoomEnter(bool isEntering, Vector2Int roomPos)
    {
        List<GridEntity> gridElements =  GridRegistry.Instance.GetRoomObjects<GridEntity>(roomPos);
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
