using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class ServiceLocator : SingletonScriptableObject<ServiceLocator>
{
    [SerializeField]
    private CameraMoverVar cameraMover;
    public CameraMover CameraMover => cameraMover.Value;

    [SerializeField]
    private GridMonoBehaviourVar cameraFocusVar;
    public GridMonoBehaviour CameraFocus => cameraFocusVar.Value;
}
