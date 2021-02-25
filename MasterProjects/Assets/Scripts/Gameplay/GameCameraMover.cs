using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraMover : CameraMover
{
    CharacterMovement movement;

    protected override void Start() 
    {
        base.Start();
        lookAtGridEntity.OnChange += OnCharChange;
        OnCharChange(null, lookAtGridEntity.Value);
    }

    protected override void OnDestroy() 
    {
        base.OnDestroy();
        lookAtGridEntity.OnChange -= OnCharChange;
    }

    private void OnCharChange(GridEntity oldVal, GridEntity newVal)
    {
        movement = newVal == null? null : newVal.GetComponent<CharacterMovement>();
    }

    public override bool CanMove(Vector2Int newPlayerPos)
    {
        return movement == null || !movement.IsGrabbed;
    }
}
