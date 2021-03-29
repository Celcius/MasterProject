using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class RoomJumper : MonoBehaviour
{
    [SerializeField]
    private RoomCollection rooms;

    private int currentRoomIndex = -1;

    [SerializeField]
    private CameraMoverVar moverVar;

    [SerializeField]
    public bool canMoveRooms = true;

    [SerializeField]
    private IInputController inputManager;

    [SerializeField]
    private BoolVar isAcceptingInput;

    [SerializeField]
    private TextBalloonVar balloonVar;

    [SerializeField]
    private GrandmaScriptVar grandmaScriptVar;

    [SerializeField]
    private FadeLabelFromStringVar[] tutLabels;

    [SerializeField]
    private SoundHelperVar soundHelper;

    [SerializeField]
    private CharacterStateVar characterState;

    [SerializeField]
    private GridEntityVar child;
    void Start()
    {
        moverVar.Value.OnCameraMoveEnd += OnCameraMoved;
        moverVar.OnChange += OnMoverVarChange;
        OnMoverVarChange(null, moverVar.Value);
    }

    private void OnDestroy() 
    {
        moverVar.Value.OnCameraMoveEnd -= OnCameraMoved;
        moverVar.OnChange -= OnMoverVarChange;    
    }

    private void OnMoverVarChange(CameraMover oldVar, CameraMover newVar)
    {
        if(newVar == null)
        {
            currentRoomIndex = -1;
            return;
        }

        OnCameraMoved(Vector2Int.zero, newVar.CurrentRoomPos);
    }

    private void OnCameraMoved(Vector2Int oldRoomPos, Vector2Int newRoomPos)
    {
        currentRoomIndex = rooms.GetIndexOfRoom(newRoomPos);
    }

    void Update()
    {
        if(currentRoomIndex < 0 || characterState.Value == CharacterState.Throwing)
        {
            return;
        }

        if(!child.Value.gameObject.activeInHierarchy)
        {
            return;
        }

        if(inputManager.IsNextRoomDown())
        {
            MoveToRoom(currentRoomIndex+1);
        }
        else if(inputManager.IsPrevRoomDown())
        {
            MoveToRoom(currentRoomIndex-1);
        }
    }

       private void MoveToRoom(int index)
    {
        if(!canMoveRooms || index == currentRoomIndex)
        {
            return;
        }
        
        moverVar.Value.TrackingEntity = false;

        soundHelper.Value.StopSound(GameSoundTag.SFX_QUAKE);
        soundHelper.Value.StopSound(GameSoundTag.SFX_LARGE_QUAKE);

        CharacterMovement movement = moverVar.Value.LookAtGridEntity.GetComponent<CharacterMovement>();
        if(movement != null)
        {
            movement.DisableColliders();
        }
        isAcceptingInput.Value = false;

        index = Mathf.Clamp(index, 0, rooms.Length);
        
        currentRoomIndex = index;
        Vector2Int prevRoom = moverVar.Value.CurrentRoomPos;
        Vector2Int room = rooms.GetRoom(index);

        moverVar.Value.MoveCameraToRoom(room, true); 
        Debug.Log("Jump to room " + index + " : " + room );
        
        Vector3 pos = moverVar.Value.transform.position;
        Vector3 entityPos = moverVar.Value.LookAtGridEntity.transform.position;
        pos.z = entityPos.z;
        moverVar.Value.LookAtGridEntity.transform.position = pos;

        RoomHandler roomHandler = moverVar.Value.GetComponent<RoomHandler>();
        roomHandler.RespawnRoom();
        grandmaScriptVar.Value.ResetGrandma(true);

        moverVar.Value.TrackingEntity = true;

        if(movement != null)
        {
            movement.EnableColliders();
        }
        if(balloonVar.Value != null)
        {
            balloonVar.Value.HideBalloon(true);
        }

        foreach(FadeLabelFromStringVar label in tutLabels)
        {
            label.Hide();
        }
        isAcceptingInput.Value = true;
    }
}
