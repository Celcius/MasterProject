using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusicOnTrigger : EntityCollideable<Boulder>
{
    [SerializeField]
    private MusicPlayer musicPlayer;

    [SerializeField]
    private MusicPlayer.MusicStyle style;
    
    [SerializeField]
    private RoomHandler roomHandler;

    [SerializeField]
    private bool shootOnce = true;

    private bool hasShot = false;

    protected virtual void Start() 
    {
        roomHandler.onEnter += OnRoomEnter;
    }

    protected virtual void OnDestroy() 
    {
        roomHandler.onEnter -= OnRoomEnter;
    }

    private void OnRoomEnter(Vector2Int roomPos)
    {
        hasShot = false;
    }

    protected override void PlayerTriggerEnter(Boulder entity) 
    {
        if(shootOnce && hasShot)
        {
            return;
        }

        musicPlayer.PlayStyle(style);
        hasShot = true;
    }
}
