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

    private void Start() 
    {
        roomHandler = CameraMover.Instance.GetComponent<RoomHandler>();
        roomHandler.onLeave += OnRoomLeave;
        hasStartedEnd = false;
        elapsed = 0;
        camTransform = CameraMover.Instance.transform;

        lastFrame = camRotation.keys[camRotation.keys.Length-1];
    }

    private void OnDestroy() 
    {
        roomHandler.onLeave -= OnRoomLeave;
    }

    protected override void PlayerTriggerEnter(CharacterMovement entity) 
    {
        isPlayerInRange = true;
        tutVar.Value = tutString;
    }

    protected override void PlayerTriggerExit(CharacterMovement entity) 
    {
        isPlayerInRange = false;
        tutVar.Value = "";
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
            }   
            return;
        }

        if(input.IsGrab())
        {
            hasStartedEnd = true;
            tutVar.Value = "";
            isAcceptingInput.Value = false;
            
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
    }
}
