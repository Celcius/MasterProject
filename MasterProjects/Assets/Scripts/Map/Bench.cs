using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;
using UnityEngine.Audio;
using System;
public class Bench : PlayerCollideable
{
    [SerializeField]
    private GridEntityVar cameraLookat;

    [SerializeField]
    private GridEntity playerBenchEntity;

    [SerializeField]
    private BoolVar isAcceptingInput;

    private bool isPlayerInRange = false;
    private bool hasStartedEnd = false;

    [SerializeField]
    private StringVar tutVar;

    [SerializeField]
    private string tutString = "Press Q to interact";

    [SerializeField]
    private IInputController input;

    private RoomHandler roomHandler;

    GridEntity cachedPlayer = null;

    [SerializeField]
    private AudioClip endClip;

    [SerializeField]
    private SoundSystem soundSystem;

    [SerializeField]
    private BoolVar canCallWithSound;

    [SerializeField]
    private GridEntity lookAtEndChild;

    [SerializeField]
    private AudioMixerSnapshot normalSnapshot;

    [SerializeField]
    private AudioMixerSnapshot endSnapshot;

    private string endClipId = "ENDCLIP";
    private void Start() 
    {
        isPlayerInRange = false;
        roomHandler = CameraMover.Instance.GetComponent<RoomHandler>();
        roomHandler.onLeave += OnRoomLeave;
    }

    private void OnDestroy() 
    {
        roomHandler.onLeave -= OnRoomLeave;
    }

    protected override void EntityTriggerEnter(CharacterMovement entity) 
    {
        isPlayerInRange = true;
        if(!hasStartedEnd)
        {
            tutVar.Value = tutString;
        }
        canCallWithSound.Value = false;
    }

    protected override void EntityTriggerExit(CharacterMovement entity) 
    {
        isPlayerInRange = false;
        tutVar.Value = "";
        canCallWithSound.Value = true;
    }

    private void LateUpdate() 
    {

        if(input.IsGrab() && isPlayerInRange)
        {
            RoomAnalytics.Instance.OnRoomLeave(CameraMover.RoomPosForWorldPos(transform.position));
            hasStartedEnd = true;
            tutVar.Value = "";
            CreditsController.Instance.StartCamRot(lookAtEndChild);
            
            cachedPlayer = cameraLookat.Value;

            GridEntity benchPlayer = Instantiate(playerBenchEntity, 
                                                cachedPlayer.transform.position, 
                                                cachedPlayer.transform.rotation);
            cameraLookat.Value = benchPlayer;

            Action<string> endAction = (string id) => {
                normalSnapshot.TransitionTo(2.0f);
            };

            soundSystem.PlaySound(endClip, endClipId, false, null, endAction);
            endSnapshot.TransitionTo(2.0f);
            cachedPlayer.gameObject.SetActive(false);
            
            
        }
    }

    private void OnRoomLeave(Vector2Int newRoom)
    {
        if(cachedPlayer != null)
        {
            cachedPlayer.gameObject.SetActive(true);
            cameraLookat.Value = cachedPlayer;
            isAcceptingInput.Value = true;
        }
        normalSnapshot.TransitionTo(0.0f);
        soundSystem.StopSound(endClipId);
    }
}
