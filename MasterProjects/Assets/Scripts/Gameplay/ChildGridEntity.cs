using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildGridEntity : CameraTargetGridEntity
{
    [SerializeField]
    private Transform representationAnchor;

    public override Vector3Int GridPos => CameraMover.GridPosForWorldPos(representationAnchor.position);
    public override  Vector2Int RoomGridPos => CameraMover.RoomPosForWorldPos(representationAnchor.position);
    
}
