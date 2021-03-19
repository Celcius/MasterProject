using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuakeCollider : RoomChangeHandler
{   
    
    [SerializeField]
    private string intendedTag = GameConstants.PLAYER_TAG;

    protected bool hasShown = false;

    [SerializeField]
    private float shakeIntensity = 0.1f;
    [SerializeField]
    private float shakeDuration = 2.0f;
    [SerializeField]
    private AnimationCurve shakeDamping;
    

    protected void OnTriggerEnter2D(Collider2D other) 
    {
        if(hasShown || other.tag != intendedTag)
        {
            return;

        }
        GameCameraMover gameCamera = (GameCameraMover) CameraMover.Instance;
        if(gameCamera != null)
        {
            gameCamera.ShakeCamera(shakeIntensity, 
                                   shakeDuration,
                                   shakeDamping, 
                                   true);
        }
        

        hasShown = true;

    }

    public override void OnRoomEnter(Vector2Int pos)
    {
        hasShown = false;
    }

    public override void OnRoomLeave(Vector2Int pos)
    {

    }
}
