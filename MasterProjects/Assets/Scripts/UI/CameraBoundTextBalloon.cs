using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AmoaebaUtils;

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

    [SerializeField]
    private AudioClip[] voiceAudio;
    
    
    [SerializeField]
    private AudioClip[] punctuationAudio;
    

    [SerializeField]
    private SoundSystem soundSystem;
    private const string voiceID = "VOICEID";
    private const string punctuationID = "PUNCTUATIONID";


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

    protected override void StringChanged(string oldVal, string newVal)
    {
       base.StringChanged(oldVal, newVal);

       if(string.IsNullOrEmpty(newVal))
       {
           return;
       }
       
       int lastIndex = newVal.Length-1;
       char last = newVal[lastIndex];
       if(char.IsWhiteSpace(last) || (IsVowel(last) && lastIndex > 0 && !IsVowel(newVal[lastIndex-1])))
       {
           return;
       }
            
        bool isPlayingVoice = soundSystem.IsPlaying(voiceID);
        bool isPlayingPunctuation = soundSystem.IsPlaying(punctuationID);

        if(char.IsPunctuation(last) && !isPlayingPunctuation)
        {
            soundSystem.StopSound(voiceID);
            soundSystem.PlaySound(punctuationAudio[Random.Range(0, punctuationAudio.Length)]);
        }

        if (char.IsLetter(last) && !isPlayingVoice)
        {
            soundSystem.StopSound(punctuationID);
            soundSystem.PlaySound(voiceAudio[Random.Range(0, voiceAudio.Length)]);
        }
    }

    public bool IsVowel(char c)
    {
        return "aeiouAEIOU".IndexOf(c) >= 0;
    }
}

