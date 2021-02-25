using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkCollider : PlayerCollideable
{
    [SerializeField]
    private TextBalloonVar grannyBalloon;

    [SerializeField]
    private TextBalloonString[] strings;

    [SerializeField]
    private RoomHandler roomHandler;

    private int curString = 0;
    private bool hasShown = false;

    private void Start() 
    {
        roomHandler.onEnter += OnEnterRoom;
        roomHandler.onLeave += OnLeaveRoom;
    }

    private void OnEnterRoom(Vector2Int newPos)
    {
        grannyBalloon.Value.OnWillHideCallback -= ShowNext;

        curString = 0;
        hasShown = false;
    }

    private void OnLeaveRoom(Vector2Int newPos)
    {
        grannyBalloon.Value.OnWillHideCallback -= ShowNext;
    }
    protected override void PlayerTriggerStay(CharacterMovement character) 
    {
        PlayerTriggerEnter(character);
    }

    protected override void PlayerTriggerEnter(CharacterMovement character) 
    {
        if(hasShown)
        {
            return;
        }

        ShowText();
        grannyBalloon.Value.OnWillHideCallback += ShowNext;
        hasShown = true;
    }

    private void OnDestroy() 
    {
        grannyBalloon.Value.OnWillHideCallback -= ShowNext;
        roomHandler.onEnter -= OnEnterRoom;
        roomHandler.onLeave -= OnLeaveRoom;
    }

    private void ShowNext()
    {
        ShowText();
    }

    private void ShowText()
    {
        if(!grannyBalloon.Value.IsLeavingOrHidden)
        {
            return;
        }

        if(curString < strings.Length && grannyBalloon.Value != null)
        {
            grannyBalloon.Value.ShowText(strings[curString], false, false);
        }
        ++curString;

        if(curString >= strings.Length)
        {
            grannyBalloon.Value.OnWillHideCallback -= ShowNext;
        }
    }
}
