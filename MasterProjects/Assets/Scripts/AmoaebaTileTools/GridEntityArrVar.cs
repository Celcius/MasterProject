using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;
public class GridEntityArrVar : ArrayVar<GridEntity> 
{
    public bool HasEntityAtPos(Vector3Int pos)
    {
        foreach(GridEntity entity in this.Value)
        {
            if(entity.GridPos == pos)
            {
                return true;
            }
        }
        return false;
    }
}
