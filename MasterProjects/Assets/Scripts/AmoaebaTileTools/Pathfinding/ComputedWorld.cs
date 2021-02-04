using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;

public class ComputedWorld : ScriptableObject
{
    [System.Serializable]
    private struct TileComputedInfo
    {
        public Vector3Int tile;
        public TileAdjacency[] adjacentPositions;

        public TileComputedInfo(Vector3Int tile, TileAdjacency[] adjacentPositions)
        {
            this.tile = tile;
            this.adjacentPositions = adjacentPositions;
        }
    }

    [System.Serializable]
    private struct TileAdjacency
    {
        public Vector3Int AdjacentPos;
        public int Cost;

        public TileAdjacency(Vector3Int adjacency, int cost)
        {
            AdjacentPos = adjacency;
            Cost = cost;
        }
    }

    [SerializeField, HideInInspector]
    Dictionary<Vector3Int, TileComputedInfo> adjacencyInfo = new Dictionary<Vector3Int, TileComputedInfo>();

    [SerializeField, HideInInspector]
    Dictionary<Vector3Int, List<Vector3Int>> inverseAdjacentLookup = new Dictionary<Vector3Int, List<Vector3Int>>();

    public void Compute(Tilemap floorMap, HashSet<Vector3Int> topPositions)
    {
        Assert.IsFalse(topPositions.Count == 0, "Empty Map being stored");

        adjacencyInfo.Clear();
        inverseAdjacentLookup.Clear();
        foreach(Vector3Int topPos in topPositions)
        {
            TileAdjacency[] adjacents = GenerateAdjacentPositions(topPos, topPositions, floorMap);
            adjacencyInfo[topPos] = new TileComputedInfo(topPos, adjacents);

            foreach(TileAdjacency adjacent in adjacents)
            {
                AddInvertAdjacentPos(adjacent.AdjacentPos, topPos);
            }
        }
    }

    private void AddInvertAdjacentPos(Vector3Int adjacent, Vector3Int origin)
    {
        if(!inverseAdjacentLookup.ContainsKey(adjacent))
        {
            inverseAdjacentLookup[adjacent] = new List<Vector3Int>();
        }

        inverseAdjacentLookup[adjacent].Add(origin);
    }

    private TileAdjacency[] GenerateAdjacentPositions(Vector3Int position, HashSet<Vector3Int> possiblePositions, Tilemap floorMap)
    {
        List<TileAdjacency> adjacents = new List<TileAdjacency>();
        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                for(int z = -1; z <= 1; z++)
                {
                    Vector3Int adjacent = position + new Vector3Int(x,y,z);

                    if(possiblePositions.Contains(adjacent))
                    {
                        int cost = GetMoveCost(position, adjacent, floorMap);
                        adjacents.Add(new TileAdjacency(adjacent, cost));
                    }
                }
            }
        }
        return adjacents.ToArray();
    }

    private int GetMoveCost(Vector3Int origin, Vector3Int destination, Tilemap floorMap)
    {
        return 1; // TODO
    }
        
    [Button]
    private void PrintWorld()
    {
        string toPrint = $"Map - {this.name}\n>>>>>>>>>>>>>Adjacents<<<<<<<<<\n";
        foreach(Vector3Int key in adjacencyInfo.Keys)
        {
            toPrint += $"{key} -> ";
            foreach(TileAdjacency adjacent in adjacencyInfo[key].adjacentPositions)
            {
                toPrint += adjacent.AdjacentPos + ",";
            }
            toPrint += "\n";
        }      
    
    toPrint += ">>>>>>>>>>>>>inverse Adjacents<<<<<<<<<\n";

        foreach(Vector3Int key in inverseAdjacentLookup.Keys)
        {
            toPrint += $"{key} -> ";
            foreach(Vector3Int inverse in inverseAdjacentLookup[key])
            {
                toPrint += inverse + ",";
            }
            toPrint += "\n";
        }   

        toPrint += ">>>>>>>>>>>>><<<<<<<<<\n";
        Debug.Log(toPrint);
    }
}

