using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTileEntity : GridEntity
{
    [SerializeField]
    private Vector3Int[] occupyOffsets;

    public override bool Occupies(Vector3Int gridPos)
    {
        if(!gameObject.activeInHierarchy)
        {
            return false;
        }

        if(this.GridPos == gridPos)
        {
            return true;
        }

        if(occupyOffsets == null || occupyOffsets.Length == 0)
        {
            return false;
        }

        foreach(Vector3Int offset in occupyOffsets)
        {
            Vector3Int pos = this.GridPos + offset;
            if(gridPos == pos)
            {
                return true;
            }
        }
        return false;
    }
}
