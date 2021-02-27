using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraBoundTextBalloon : TextBalloon
{
    private CameraMover mover = null;
    private Vector3 localPos;

    [SerializeField]
    private RectTransform minAnchor;

    [SerializeField]
    private RectTransform maxAnchor;

    [SerializeField]
    private Image[] quadrant1Images;

    [SerializeField]
    private Image[] quadrant2Images;

    [SerializeField]
    private Image[] quadrant3Images;

    [SerializeField]
    private Image[] quadrant4Images;

    float minHOffset;
    float maxHOffset;
    bool hasStarted = false;
    protected override void Start() 
    {
        mover = CameraMover.Instance;
        base.Start();
        localPos = rectTransform.localPosition;
        minHOffset = minAnchor.localPosition.x;
        maxHOffset = maxAnchor.localPosition.x;
        hasStarted = true;
    }    

    private void LateUpdate() 
    {
        ClampCameraPos(rectTransform.sizeDelta);    
    }

    private void ClampCameraPos(Vector2 size)
    {
        if(!hasStarted)
        {
            return; 
        }
        Vector2 minPos = (Vector2)localPos - size /2.0f;
        Vector2 maxPos = (Vector2)localPos + size /2.0f;
        
        Bounds bounds = mover.ViewportBounds;        
        Vector2 minLocalBounds = bounds.min - transform.parent.position - Vector3.right * minHOffset;
        Vector2 maxLocalBounds = bounds.max - transform.parent.position - Vector3.right * maxHOffset;

        Vector2 minVec = Vector2.Max(minPos, minLocalBounds);
        Vector2 maxVec = Vector2.Min(maxPos, maxLocalBounds);
        minVec = minVec - minPos;
        maxVec = maxVec - maxPos;

    

        Vector3 goalPos = localPos + (Vector3)minVec + (Vector3)maxVec;
        rectTransform.localPosition = goalPos;
    
     /*   Vector3 worldGoalPos =  transform.parent.position + (Vector3)goalPos;
        Debug.DrawLine(bounds.min, bounds.max, Color.red, 1.0f);
        Debug.DrawLine(transform.position + (Vector3)minPos, transform.position + (Vector3)maxPos, Color.green, 0.5f);
        Debug.DrawLine(worldGoalPos + (Vector3)minPos - localPos,worldGoalPos +(Vector3)maxPos -localPos, Color.cyan, 0.5f);
*/

        UpdateQuadrantImages(quadrant1Images, Vector3.one);
        UpdateQuadrantImages(quadrant2Images, -Vector3.right + Vector3.up);
        UpdateQuadrantImages(quadrant3Images, -Vector3.one);
        UpdateQuadrantImages(quadrant4Images, Vector3.right - Vector3.up);

    }

    private void UpdateQuadrantImages(Image[] images, Vector2 expectedQuadrant)
    {
        Vector2 quadrant = (transform.position - transform.parent.position).normalized;
        bool isRightQuadrant = Mathf.Sign(expectedQuadrant.x) == Mathf.Sign(quadrant.x) &&
                               Mathf.Sign(expectedQuadrant.y) == Mathf.Sign(quadrant.y);

        foreach(Image rend in images)
        {
            rend.gameObject.SetActive(isRightQuadrant);
        }
    }

    protected override void UpdateRectSize(Vector2 size)
    {
        base.UpdateRectSize(size);
        
        if(!IsShowing)
        {
            return;
        }
        
        ClampCameraPos(size);
    }
}

