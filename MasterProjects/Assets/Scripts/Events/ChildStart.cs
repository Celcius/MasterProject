using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildStart : RoomChangeHandler
{
    
    [SerializeField]
    private GridEntityVar childEntity;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private BoolVar inputActive;

    [SerializeField]
    private RoomCollection roomOrder;

    
    [SerializeField]
    private string tutorialLabel = "W A S D to move";
    
    [SerializeField]
    private StringVar tutorialStringVar;
    

    public override void OnRoomEnter(Vector2Int pos)
    {
        if(!roomOrder.ContainsRoom(pos) || roomOrder.GetIndexOfRoom(pos) != 0)
        {
            return;
        }

        childEntity.Value.transform.gameObject.SetActive(false);
        animator.SetTrigger("Restart");
        inputActive.Value = false;
    }

    public void OnAnimEnterEnd()
    {
        childEntity.Value.transform.gameObject.SetActive(true);
        inputActive.Value = true;
        ShowTutorialString();
    }

    public override void OnRoomLeave(Vector2Int pos)
    {

    }


    public void ShowTutorialString()
    {
        tutorialStringVar.Value = tutorialLabel;
    }
}
