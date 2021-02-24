using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System;
using AmoaebaUtils;


[RequireComponent(typeof(PixelPerfectCamera))]
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

    [SerializeField]
    private GridEntityArrVar moveWithRoomEntities;

    public Vector2Int CurrentRoomPos => playerRoom;

    private IEnumerator cameraShake;

    private static CameraMover moverSingleton;
    public static CameraMover Instance => moverSingleton;

    public Vector3 CellSize => grid.cellSize;

    private int currentIndex = 0;

    [SerializeField]
    private GridEntityVar lookAtGridEntity;
    public GridEntity LookAtGridEntity => lookAtGridEntity.Value;

    public Action<Vector2Int, Vector2Int> OnCameraMoveStart;
    
    public Action<Vector2Int, Vector2Int> OnCameraMoveEnd;
    [SerializeField]
    private PixelPerfectCamera pixelCamera;

    private Vector2Int playerRoom;

    [SerializeField]
    private float shakeDuration = 0.5f;
    [SerializeField]
    private float shakeMagnitude = 0.5f;
    [SerializeField]
    private float shakeDampingSpeed = 0.1f;
    
    public bool TrackingEntity = true;

    public Vector2 RefResolution 
    {
        get { return new Vector2(pixelCamera.refResolutionX, pixelCamera.refResolutionY);}
    }

    public float PixelsPerUnit => pixelCamera.assetsPPU;

    public Vector2 RoomSize => new Vector2(
            (int)(pixelCamera.refResolutionX / pixelCamera.assetsPPU),
            (int)(pixelCamera.refResolutionY / pixelCamera.assetsPPU));

    private bool moving = false;
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

    private void OnDestroy() 
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

        if((int)playerRoom.x != (int)newPlayerPos.x || 
           (int)playerRoom.y != (int)newPlayerPos.y)
           {
               MoveCameraToRoom(newPlayerPos);
           }
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
        if(cameraShake != null)
        {
            StopCoroutine(cameraShake);
        }
        cameraShake = null;
        Vector2Int camRoomPos = GridUtils.RoomPosForWorldPos(transform.position, this.RoomSize, (Vector2)moverSingleton.CellSize);
        if(instant)
        {
            transform.position = GridUtils.WorldPosForRoomPos(newCamRoom, this.RoomSize, (Vector2)moverSingleton.CellSize, transform.position.z);    
            playerRoom =  newCamRoom;
            OnCameraMoveEnd?.Invoke(camRoomPos, playerRoom);
            ReorderRoomCamEntities(camRoomPos);
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
        ReorderRoomCamEntities(startRoomPos);


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

    private void ReorderRoomCamEntities(Vector2Int prevRoomPos)
    {
        foreach(GridEntity entity in moveWithRoomEntities.Value)
        {
            if(entity != null)
            {
                GridRegistry.Instance.ReorderRoomGridObject(entity, 
                                                            prevRoomPos);
            }
        }
    }

    public Vector3 GetTargetCameraPosition()
    {
        Vector2Int targetRoom = LookAtGridEntity.RoomGridPos;
        return GridUtils.WorldPosForRoomPos(targetRoom, this.RoomSize,(Vector2)moverSingleton.CellSize, transform.position.z);
    }

    public void ShakeCamera(float intensity)
    {
        ShakeCamera(intensity, intensity * shakeDuration, intensity * shakeDampingSpeed);
    }
    public void ShakeCamera(float intensity, float duration, float damping = 1.0f)
    {
        if(moving)
        {
            return;
        }

        transform.position = GetTargetCameraPosition();
        cameraShake = CameraShake(duration,
                                  intensity * shakeMagnitude,
                                  damping);
        StartCoroutine(cameraShake);
    }
    private IEnumerator CameraShake(float time, float magnitude, float damping)
    {
        Vector3 initialPosition = GetTargetCameraPosition();
        float duration = time;
        while(duration >= 0)
        {
            transform.localPosition = initialPosition + UnityEngine.Random.insideUnitSphere * magnitude;
        
            duration -= Time.deltaTime * damping;
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
