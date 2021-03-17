using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class GameRoomHandler : RoomHandler
{
    [SerializeField]
    private GridEntityVar playerEntity;

    [SerializeField]
    private GrandmaScriptVar grandmaEntity;

    private CharacterMovement characterMovement;

    [SerializeField]
    private TextBalloonVar balloonVar;


    [SerializeField]
    private FadeLabelFromStringVar[] tutLabels;

    protected override void Start() 
    {
        base.Start();
        characterMovement = playerEntity.Value.GetComponent<CharacterMovement>();
        CameraMover.Instance.OnCameraMoveStart += OnMoveStart;
    }

    private void OnMoveStart(Vector2Int oldRoom, Vector2Int newRoom)
    {
        foreach(FadeLabelFromStringVar label in tutLabels)
        {
            label.Hide();
        }

        if(balloonVar.Value != null)
        {
            balloonVar.Value.HideBalloon(true);
        }
    }


    public override void RespawnRoom()
    {
        base.RespawnRoom();
        characterMovement.ResetCharacter(true);
        grandmaEntity.Value.ResetGrandma(true);
    }
}
