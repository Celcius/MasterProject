using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEntity : MonoBehaviour
{
    public virtual Vector3Int GridPos => CameraMover.GridPosForWorldPos(transform.position);
    public virtual  Vector2Int RoomGridPos => CameraMover.RoomPosForWorldPos(transform.position);
    
    protected Vector3 originalPosition;

    [SerializeField]
    private bool isBlocking = true;
    public bool IsBlocking => isBlocking;

    [SerializeField]
    private bool allowsStand = true;
    public bool AllowsStand => allowsStand;

    [SerializeField]
    private bool isRoomStatic = true;
    public bool IsRoomStatic => isRoomStatic;

    protected virtual void Start()
    {
        if(IsRoomStatic)
        {
            GridRegistry.Instance.AddRoomGridObject(this);
        }
        else
        {
            GridRegistry.Instance.AddNonRoomGridObject(this);
        }
        
        originalPosition = transform.position;        
    }

    protected virtual void OnDestroy()
    {
        if(GridRegistry.Instance != null)
        {
            if(IsRoomStatic)
            {
                GridRegistry.Instance.RemoveRoomGridObject(this);
            }
            else
            {
                GridRegistry.Instance.RemoveNonRoomGridObject(this);
            }
        }
    }

    public virtual void OnRoomWillEnter() 
    {
        this.gameObject.SetActive(true);
        this.transform.position = originalPosition;
    }

    public virtual void OnRoomWillLeave()  {}

    public virtual void OnRoomEnter() {}

    public virtual void OnRoomLeave() 
    {
        this.gameObject.SetActive(false);
    }

    public virtual bool Occupies(Vector3Int gridPos)
    {
        return gameObject.activeInHierarchy && this.GridPos == gridPos;
    }
}
