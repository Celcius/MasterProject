using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisiblePosSetterGridEntity : GridEntity
{
    [SerializeField]
    private Vector2Var var;

    [SerializeField]
    private bool setGridPos = false;
    private void OnEnable() 
    {
        if(setGridPos)
        {
            var.Value = CameraMover.WorldPosForGridPos(GridPos, 0);
        }
        else
        {
            var.Value = transform.position - CameraMover.Instance.CellSize /2.0f;
        }
    }
}
