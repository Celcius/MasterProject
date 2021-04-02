using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AmoaebaUtils;

[RequireComponent(typeof(Image))]
public class RestartController : MonoBehaviour
{
    private Image image;
    
    [SerializeField]
    private CharacterStateVar characterVar;

    [SerializeField]
    private float timeToReset;

    [SerializeField]
    private AnimationCurve fadeOutAnimation;


    [SerializeField]
    private float returnSpeed;

    private IEnumerator fadeRoutine = null;

    [SerializeField]
    private BoolVar isAcceptingInput;

    [SerializeField]
    private float activateInputRatio = 0.5f;

    [SerializeField]
    private FadeLabelFromStringVar[] labels;

    [SerializeField]
    private GridEntityVar characterEntityVar;

    [SerializeField]
    private RoomAnalytics roomAnalytics;

    private bool waitOnInput = false;

    private void Start()
    {
        image = GetComponent<Image>();
        characterVar.OnChange += OnChange;
        fadeRoutine = FadeIn(true, 0.5f);
        StartCoroutine(fadeRoutine);
    }

    private void OnDestroy() 
    {
        characterVar.OnChange -= OnChange;
    }

    private void OnChange(CharacterState oldState, CharacterState newState)
    {
        if(oldState == newState)
        {
            return;
        }
        
        if(oldState == CharacterState.Crying)
        {
            if(fadeRoutine != null)
            {
                StopCoroutine(fadeRoutine);
            }
            fadeRoutine = FadeIn();
            StartCoroutine(fadeRoutine);
        }

        if(newState == CharacterState.Crying)
        {
            if(fadeRoutine != null)
            {
                StopCoroutine(fadeRoutine);
            }
            fadeRoutine = FadeOutAndReset();
            StartCoroutine(fadeRoutine);
        }
    }

    private IEnumerator FadeOutAndReset()
    {        
        Color color = image.color;
        float ratio = color.a;
        float elapsed = 0.0f;

        while(elapsed <= timeToReset)
        {
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
            ratio = Mathf.Clamp01(elapsed/ timeToReset);
            color.a = fadeOutAnimation.Evaluate(ratio);
            image.color = color;
        }

        color.a = 1.0f;
        image.color = color;
        RespawnRoom();
        fadeRoutine = null;
        waitOnInput = true;
        isAcceptingInput.Value = false;
    }
    
    private IEnumerator FadeIn(bool restartAlpha = false, float timeDilation = 1.0f)
    {
        Color color = image.color;
        if(restartAlpha)
        {
            color.a = 1.0f;
            image.color = color;
        }
        float ratio = color.a;
        
        while(ratio >= 0)
        {
            yield return new WaitForEndOfFrame();
            ratio = Mathf.Max(0, ratio - returnSpeed * Time.deltaTime * timeDilation);

            if(waitOnInput && ratio <= activateInputRatio)
            {
                waitOnInput = false;
            isAcceptingInput.Value = true;
            }
            color.a = ratio;
            image.color = color;
        }
        color.a = 0;
        image.color = color;
        fadeRoutine = null;
        
    }

    private void RespawnRoom()
    {
        roomAnalytics.RetryRoom(characterEntityVar.Value.RoomGridPos);
        foreach(FadeLabelFromStringVar label in labels)
        {
            label.Hide();
        }
        RoomHandler room = CameraMover.Instance.GetComponent<RoomHandler>();
        room.RespawnRoom();
    }

}
