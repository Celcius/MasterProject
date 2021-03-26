using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AmoaebaUtils;

public class DarknessController : RoomChangeHandler
{
    [SerializeField]
    private RoomCollection fullDarkRoomCollection;
    
    [SerializeField]
    private float fullDarkRadius = 0.25f;
    
    [SerializeField]
    private RoomCollection halfDarkRoomCollection;

    [SerializeField]
    private float halfDarkRadius = 0.7f;
    
    
    [SerializeField]
    private Image image;

    [SerializeField]
    private GridEntityVar playerEntity; 

    private float goalRadius = 0.0f; 
    private float curRadius = 0.0f;

    [SerializeField]
    private float fadeSpeed = 0.5f;

    private Vector3 updatedPos;

    private Material sharedMat;
    
    private Camera cam;

    private GridEntity childEntity;

    private float startSpeed = 0.5f;
    protected override void Start() 
    {
        startSpeed = fadeSpeed;
        base.Start();
        sharedMat = image.material;
        cam = Camera.main;
        SetRadius(1.0f);
    }

    void FixedUpdate()
    {
        if(childEntity == null && playerEntity.Value == null)
        {
            return;
        }
        else if(childEntity == null)
        {
            childEntity = playerEntity.Value;
        }

        SetPosition(childEntity.transform.position);
        if(curRadius != goalRadius)
        {
            float dist = goalRadius - curRadius;
            float delta = fadeSpeed * Time.deltaTime;
            if(Mathf.Abs(dist) <= delta)
            {
                SetRadius(goalRadius);
            }
            else
            {
                SetRadius(curRadius + Mathf.Sign(dist) * delta);
            }
        }
    }
    public void SetChild(GridEntity newChild)
    {
        childEntity = newChild;
    }
    
    public override void OnRoomEnter(Vector2Int pos)
    {
        fadeSpeed = startSpeed;

        if(halfDarkRoomCollection.ContainsRoom(pos))
        {
            goalRadius = halfDarkRadius;
        }
        else if(fullDarkRoomCollection.ContainsRoom(pos))
        {
            goalRadius = fullDarkRadius;
        }
        else
        {
            goalRadius = 1.0f;
        }
    }

    public void GoToHalfDarkRadius()
    {
        goalRadius = halfDarkRadius;
    }

    public void GoToFullDarkRadius()
    {
        goalRadius = fullDarkRadius;
    }

    public void GoToNoDarkRadius()
    {
        goalRadius = 1.0f;
    }



    public override void OnRoomLeave(Vector2Int pos) {}

    private void SetRadius(float radius)
    {
        if(sharedMat == null)
        {
            sharedMat = image.material;
        }
        curRadius = radius;
        
        if(radius > halfDarkRadius)
        {
            float lerpedAlpha = 1.0f - ((radius-halfDarkRadius) / (1.0f - halfDarkRadius));
            ShaderInstanceUtils.SetDarknessOpacity(sharedMat, lerpedAlpha);
        }
        ShaderInstanceUtils.SetDarknessRadius(sharedMat, curRadius);
    }

    private void SetPosition(Vector3 worldPos)
    {
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
        Bounds ortoBounds = UnityEngineUtils.CameraOrthographicViewportBounds(cam);
        float xPos = screenPos.x / Screen.width ;
        float yPos = 1.0f-screenPos.y / Screen.height;

        ShaderInstanceUtils.SetDarknessMaterialPosition(xPos, yPos, sharedMat);
    }

    public void FadeTo(float goalVal, float speed)
    {
        this.fadeSpeed = speed;
        goalRadius = goalVal;
    }

#if UNITY_EDITOR
    private void OnValidate() 
    {
        if(!Application.isPlaying)
        {
            SetRadius(1.0f);
        }
    }
#endif
}
