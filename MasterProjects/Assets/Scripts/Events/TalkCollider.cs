using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkCollider : EntityCollideable<Component>
{
    [SerializeField]
    private TextBalloonVar grannyBalloon;

    [SerializeField]
    private TextBalloonString[] strings;

    [SerializeField]
    private RoomHandler roomHandler;

    [SerializeField]
    private GrandmaScriptVar grandmaVar;

    private int curString = 0;
    private bool hasShown = false;

    [SerializeField]
    private bool stopGrandmother = false;

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
    protected override void EntityTriggerStay(Component comp) 
    {
        EntityTriggerEnter(comp);
    }

    protected override void EntityTriggerEnter(Component comp) 
    {
        if(hasShown)
        {
            return;
        }

        ShowText();
        grannyBalloon.Value.OnWillHideCallback += ShowNext;
        hasShown = true;

        if(stopGrandmother)
        {
            GrandmaController controller = grandmaVar.Value.GetComponent<GrandmaController>();
            if(controller != null)
            {
                controller.SetState(GrandmaStateEnum.Idle);
            }
        }
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
