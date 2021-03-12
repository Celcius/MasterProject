using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDarknessOnBoulderCollision : EntityCollideable<Boulder>
{
    [SerializeField]
    private DarknessController darknessController;

    [SerializeField, Range(0,1)]
    private float goalVal;

    [SerializeField]
    private float goalSpeed = 2.0f;

    protected override void EntityTriggerEnter(Boulder entity) 
    {
        darknessController.FadeTo(goalVal, goalSpeed);
    }

}
