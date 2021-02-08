using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoomHandler : RoomHandler
{
    [SerializeField]
    private GridEntityVar playerEntity;

    [SerializeField]
    private GrandmaScriptVar grandmaEntity;

    private CharacterMovement characterMovement;

    protected override void Start() 
    {
        base.Start();
        characterMovement = playerEntity.Value.GetComponent<CharacterMovement>();
    }

    public override void RespawnRoom()
    {
        base.RespawnRoom();
        characterMovement.ResetCharacter(true);
        grandmaEntity.Value.ResetGrandma(true);
    }
}
