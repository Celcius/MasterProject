using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  AmoaebaUtils;

public class VerticalWorldPathfindingAdapter : AStarMapFeeder<Vector3Int>
{
    private VerticalWorldController worldController;
    
    private VerticalWorldPathfindingAdapter() {}

    public VerticalWorldPathfindingAdapter(VerticalWorldController worldController) 
    {
        this.worldController = worldController;
    }

    private AStarSearch<Vector3Int> search = new AStarSearch<Vector3Int>();
    private bool considerOccupiedPos = false;
    
    public void PerformPathSearch(Vector3Int origin, 
                                  Vector3Int destination, 
                                  Action<Vector3Int[]> callback = null,
                                  bool considerOccupiedPos = false)
    {
        search.ClearSearch();
        this.considerOccupiedPos = considerOccupiedPos;
        
        Debug.Log($"{origin} -> {destination}");
        search.PerformSearchAsync(origin, destination, this, callback);
    }

    public Vector3Int[] GetNeighbours(Vector3Int pos)
    {
        HashSet<Vector3Int> topPositions = worldController.TopPositions;
        List<Vector3Int> neighbours = new List<Vector3Int>();

        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {  
                if(Mathf.Abs(i) == Mathf.Abs(j))
                {
                    continue;
                }
                Vector3Int newPos =  pos + new Vector3Int(i,j,pos.z);
                if(topPositions.Contains(newPos) 
                    && (!considerOccupiedPos || !IsOccupied(newPos)))
                {   
                    neighbours.Add(newPos);
                }
            
            }
        
        }
        return neighbours.ToArray();
    }

    private bool IsOccupied(Vector3Int pos)
    {
        GridEntity[] entities = GridRegistry.Instance.GetEntitiesAtPos(pos);
        if(entities == null)
        {
            return false;
        }
        foreach(GridEntity entity in entities)
        {
            if(entity.IsBlocking)
            {
                return true;
            }
        }
        return false;
    }

    public float GetMoveCost(Vector3Int origin, Vector3Int dest)
    {
        float zOffset = Mathf.Abs(dest.z - origin.z); 
        float offset = Mathf.Abs(dest.x - origin.x) 
                    + Mathf.Abs(dest.y - origin.y);
        return (offset <= 1) && (zOffset <= 1)? offset : float.MaxValue;
    }
    
    public float GetDistanceEstimation(Vector3Int origin, Vector3Int dest)
    {
         return Vector3Int.Distance(origin, dest);
    }

    public bool SameNode(Vector3Int node1, Vector3Int node2)
    {   
        return node1.Equals(node2);
    }

}
