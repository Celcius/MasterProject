﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

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
    private AnimationCurve camRotation;

    private Transform camTransform;

    private float elapsed = 0;
    private Keyframe lastFrame;

    [SerializeField]
    private AudioClip endClip;

    [SerializeField]
    private SoundSystem soundSystem;

    [SerializeField]
    private BoolVar canCallWithSound;

    [SerializeField]
    private GridEntity lookAtEndChild;

    private string endClipId = "ENDCLIP";
    private bool hasStartedCredits = false;

    private void Start() 
    {
        isPlayerInRange = false;
        roomHandler = CameraMover.Instance.GetComponent<RoomHandler>();
        roomHandler.onLeave += OnRoomLeave;
        hasStartedEnd = false;
        elapsed = 0;
        camTransform = CameraMover.Instance.transform;

        lastFrame = camRotation.keys[camRotation.keys.Length-1];
        hasStartedCredits = false;
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
        if(hasStartedEnd)
        {
            if(elapsed < lastFrame.time)
            {
                camTransform.rotation = Quaternion.Euler(camRotation.Evaluate(elapsed),0,0);
                elapsed += Time.deltaTime;
            }
            else
            {
                camTransform.rotation = Quaternion.Euler(lastFrame.value,0,0);
                if(!hasStartedCredits)
                {
                    CreditsController.Instance.StartMoveCredits(lookAtEndChild);                    
                    hasStartedCredits = true;
                }
            }   
            return;
        }

        if(input.IsGrab() && isPlayerInRange)
        {
            hasStartedEnd = true;
            tutVar.Value = "";
            isAcceptingInput.Value = false;
            tutVar.Value = "";
            soundSystem.PlaySound(endClip, endClipId, false, null);
            
            cachedPlayer = cameraLookat.Value;

            GridEntity benchPlayer = Instantiate(playerBenchEntity, 
                                                cachedPlayer.transform.position, 
                                                cachedPlayer.transform.rotation);
            cameraLookat.Value = benchPlayer;

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

        soundSystem.StopSound(endClipId);
    }
}
