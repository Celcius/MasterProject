using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class GrandmaPathFeeder : MonoBehaviour, AStarMapFeeder<Vector2Int>
{

 
    [SerializeField]
    private RoomTileController roomController;

    public Vector2Int[] GetNeighbours(Vector2Int pos)
    {
        List<Vector2Int> retVal = new List<Vector2Int>();
        Vector2Int[] neighbours = roomController.GetNeighbours(pos, PathPredicate);
        foreach(Vector2Int neighbour in neighbours)
        {
            GridEntity[] entities = GridRegistry.Instance.GetEntitiesAtPos((Vector3Int)neighbour);
            bool canStep = true;
            if(entities != null)
            {
                foreach(GridEntity entity in entities)
                {
                    Crack crack = entity.GetComponent<Crack>();
                    if(crack != null && crack.IsHole)
                    {
                        canStep = false;
                        break;
                    }
                }
            }

            if(canStep)
            {
                retVal.Add(neighbour);
            }
        }
        return retVal.ToArray();
    }
    
    public float GetMoveCost(Vector2Int origin, Vector2Int dest)
    {
        return roomController.GetMoveCost(origin, dest);
    }
    
    public float GetDistanceEstimation(Vector2Int origin, Vector2Int dest)
    {
        return roomController.GetDistanceEstimation(origin, dest);
    }

    public bool SameNode(Vector2Int node1, Vector2Int node2)
    {
        return roomController.SameNode(node1, node2);
    }
    private bool PathPredicate(GridEntity entity)
    {
        return entity == transform;
    }


}
