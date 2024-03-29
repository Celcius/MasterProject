﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System;
using AmoaebaUtils;
using UnityEngine.Experimental.Rendering;

public class CameraMover : MonoBehaviour
{
    [SerializeField]
    private Vector2Int[] cameraPositions;
    
    [SerializeField]
    private float moveTime = 2.5f;
    
    [SerializeField]
    private AnimationCurve moveCurve;

    [SerializeField]
    private Grid grid;

    public Vector2Int CurrentRoomPos => playerRoom;

    protected IEnumerator cameraShake;

    private static CameraMover moverSingleton;
    public static CameraMover Instance => moverSingleton;

    public Vector3 CellSize => grid.cellSize;

    private int currentIndex = 0;

    [SerializeField]
    protected GridEntityVar lookAtGridEntity;
    public GridEntity LookAtGridEntity => lookAtGridEntity.Value;

    public Action<Vector2Int, Vector2Int> OnCameraMoveStart;
    
    public Action<Vector2Int, Vector2Int> OnCameraMoveEnd;
    
    [SerializeField]
    private Vector2 refResolution;

    [SerializeField]
    private float assetsPPU;
    

    [SerializeField]
    private Camera cam;

    private Vector2Int playerRoom;

    [SerializeField]
    protected float shakeDuration = 0.5f;
    [SerializeField]
    protected float shakeMagnitude = 0.5f;
    [SerializeField]
    protected float shakeDampingSpeed = 0.1f;
    
    public bool TrackingEntity = true;

    public Bounds ViewportBounds 
    {
        get 
        {
            if(cam == null)
            {
                return default(Bounds);
            }

           return UnityEngineUtils.CameraOrthographicViewportBounds(cam);
        }
    }

    public Vector2 RefResolution 
    {
        get { return refResolution;}
    }

    public float PixelsPerUnit => assetsPPU;

    public Vector2 RoomSize => new Vector2(
            (int)(refResolution.x / assetsPPU),
            (int)(refResolution.y / assetsPPU));

    private bool moving = false;
    public bool Moving => moving;
    private bool isFirstUpdate = true;
    private void Awake()
    {   
        if(moverSingleton != null && moverSingleton != this)
        {
            Debug.LogError($"There should only be one Camera Mover ({moverSingleton} VS {this})");
        }
        
        moverSingleton = this;
    }

    private void OnEnable()
    {
        if(moverSingleton != null && moverSingleton != this)
        {
            Debug.LogError($"There should only be one Camera Mover ({moverSingleton} VS {this})");
        }
        
        moverSingleton = this;
    }

    protected virtual void Start()
    {

    }

    protected virtual void OnDestroy() 
    {
        moverSingleton = null;    
    }

    private void FixedUpdate()
    {
        if(!UnityEngineUtils.IsInPlayModeOrAboutToPlay())
        {
            return;
        }

        if(isFirstUpdate)
        {
            Vector2Int roomPos = GridUtils.RoomPosForWorldPos(LookAtGridEntity.transform.position, this.RoomSize, (Vector2)moverSingleton.CellSize);
            playerRoom = roomPos;
            transform.position =  GridUtils.WorldPosForRoomPos(roomPos, this.RoomSize, (Vector2)moverSingleton.CellSize, transform.position.z);
            isFirstUpdate = false;
            OnCameraMoveEnd?.Invoke(new Vector2Int(0,0), roomPos);
        }

        if(TrackingEntity && LookAtGridEntity != null)
        {        
            Vector2Int lookAtPos = LookAtGridEntity.RoomGridPos;
            LookAtTransformMoved(lookAtPos);
        }
    }

    public void LookAtTransformMoved(Vector2Int newPlayerPos)
    {
        if(moving)
        {
            return;
        }

        if(CanMove(newPlayerPos) && 
           ((int)playerRoom.x != (int)newPlayerPos.x || 
           (int)playerRoom.y != (int)newPlayerPos.y))
           {
               MoveCameraToRoom(newPlayerPos);
           }
    }
    
    public virtual bool CanMove(Vector2Int newPlayerPos)
    {
        return true;
    }

    public void NextPosition()
    {
        
        int nextIndex = (currentIndex+1)%cameraPositions.Length;
        
        Action onEnd = () =>
        {
            currentIndex = nextIndex;
        };

        StartCoroutine(MoveCameraTo(
                       cameraPositions[currentIndex], 
                       cameraPositions[nextIndex],
                       onEnd));
    }

    public void MoveCameraToRoom(Vector2Int newCamRoom, bool instant = false)
    {
        StopShake();
        cameraShake = null;
        Vector2Int camRoomPos = GridUtils.RoomPosForWorldPos(transform.position, this.RoomSize, (Vector2)moverSingleton.CellSize);
        if(instant)
        {
            transform.position = GridUtils.WorldPosForRoomPos(newCamRoom, this.RoomSize, (Vector2)moverSingleton.CellSize, transform.position.z);    
            playerRoom =  newCamRoom;
            OnCameraMoveEnd?.Invoke(camRoomPos, playerRoom);
        }
        else
        {
            StartCoroutine(
            MoveCameraTo(camRoomPos,
                       newCamRoom,
                       () => {
                           playerRoom =  newCamRoom;
                       }));
        }
        
    }

    private IEnumerator MoveCameraTo(Vector2Int startRoomPos, Vector2Int endRoomPos, Action onMoveEnd = null)
    {
        if(moving)
        {
            yield break;
        }

        moving = true;

        OnCameraMoveStart?.Invoke(startRoomPos, endRoomPos);

        Vector3 startPosition = GridUtils.WorldPosForRoomPos(startRoomPos, this.RoomSize, (Vector2)moverSingleton.CellSize, transform.position.z);
        Vector3 targetPosition = GridUtils.WorldPosForRoomPos(endRoomPos, this.RoomSize, (Vector2)moverSingleton.CellSize, transform.position.z);
 
        float time = moveTime;
        float instant = 0.0f;
        while(time > 0)
        {
            time -= Time.deltaTime;
            instant = 1.0f-(time/moveTime);
            transform.position = Vector3.Lerp(startPosition, targetPosition, moveCurve.Evaluate(instant));
            yield return new WaitForEndOfFrame();
        }

        transform.position = targetPosition;
        moving = false;
        OnCameraMoveEnd?.Invoke(startRoomPos, endRoomPos);

        if(onMoveEnd != null)
        {
            onMoveEnd();
        }
    }

    public Vector3 GetTargetCameraPosition()
    {
        Vector2Int targetRoom = LookAtGridEntity.RoomGridPos;
        return GridUtils.WorldPosForRoomPos(targetRoom, this.RoomSize,(Vector2)moverSingleton.CellSize, transform.position.z);
    }

    public virtual void ShakeCamera(float intensity)
    {
        ShakeCamera(intensity, intensity * shakeDuration, intensity * shakeDampingSpeed);
    }
    public virtual void ShakeCamera(float intensity, float duration, float damping = 1.0f)
    {
        AnimationCurve dampingCurve = new AnimationCurve();
        dampingCurve.AddKey(0, damping);
        dampingCurve.AddKey(1.0f, damping);
        ShakeCamera(intensity, duration, dampingCurve);
    }

    public virtual void ShakeCamera(float intensity, float duration, AnimationCurve damping)
    {
        if(moving)
        {
            return;
        }

        transform.position = GetTargetCameraPosition();
        StopShake();
        cameraShake = CameraShake(duration,
                                  intensity * shakeMagnitude,
                                  damping);
        StartCoroutine(cameraShake);
    }

    protected virtual void StopShake()
    {
        if(cameraShake != null)
        {
            StopCoroutine(cameraShake);
        }
    }

    protected virtual IEnumerator CameraShake(float time, float magnitude, AnimationCurve damping)
    {
        Vector3 initialPosition = GetTargetCameraPosition();
        float duration = time;
        while(duration >= 0)
        {
            float ratio = Mathf.Clamp01((time-duration) / time);
            float dampingVal = damping.Evaluate(ratio);

            transform.localPosition = initialPosition + UnityEngine.Random.insideUnitSphere * magnitude* dampingVal;
            
            duration -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        transform.position = GetTargetCameraPosition();
    }

    public static Vector2Int RoomPosForWorldPos(Vector3 worldPos)
    {
        return GridUtils.RoomPosForWorldPos(worldPos, moverSingleton.RoomSize, (Vector2)moverSingleton.CellSize);
    } 

    public static Vector2Int RoomPosForGridPos(Vector3Int gridPos)
    {
        return GridUtils.RoomPosForGridPos(gridPos, (Vector2)moverSingleton.CellSize, moverSingleton.RoomSize);
    } 

    public static Vector3Int GridPosForWorldPos(Vector3 worldPos)
    {
        return GridUtils.GridPosForWorldPos(worldPos, (Vector2)moverSingleton.CellSize);
    }

    public static Vector3 WorldPosForGridPos(Vector3Int gridPos, float zPos)
    {
        return GridUtils.WorldPosForGridPos(gridPos, zPos, (Vector2)moverSingleton.CellSize);
    }
}
