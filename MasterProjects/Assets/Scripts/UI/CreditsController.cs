using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using AmoaebaUtils;

public class CreditsController : Singleton<CreditsController>
{
    Vector2 startPos;
    
    [SerializeField]
    Vector2 endPos;

    [SerializeField]
    private float timeToMove;

    [SerializeField]
    private RectTransform creditsTransform;

    [SerializeField]
    private TextMeshProUGUI finLabel;
    
    [SerializeField]
    private float fadeInFinTime = 2.0f;

    private IEnumerator moveRoutine;

    [SerializeField]
    private float finalFadeDuration = 5.0f;

    [SerializeField]
    private DarknessController darknessController;

    [SerializeField]
    private Transform grid;

    [SerializeField]
    private Transform[] endImages;

    
    [SerializeField]
    private AnimationCurve camRotation;

    [SerializeField]
    private Material sunMaterial;

    [SerializeField]
    private AnimationCurve sunRotation;

    private Transform camTransform;

    private float elapsed = 0;
    private Keyframe lastFrame;
    
    private bool hasStartedCredits = false;

    private GridEntity lookAtEndChild;

    
    [SerializeField]
    private BoolVar isAcceptingInput;
    private bool hasStartedEnd = false;

    void Start()
    {
        SetSunOffset(0.0f);
        startPos = creditsTransform.anchoredPosition;
        moveRoutine = null;
        TextMeshProUGUI[] labels = GetComponentsInChildren<TextMeshProUGUI>();
        foreach(TextMeshProUGUI label in labels)
        {
            Color col = label.color;
            col.a = 1.0f;
            label.color = col;
        }
        Color c = finLabel.color;
        c.a = 0;
        finLabel.color = c;

        hasStartedEnd = false;
        elapsed = 0;
        camTransform = CameraMover.Instance.transform;

        lastFrame = camRotation.keys[camRotation.keys.Length-1];
        hasStartedCredits = false;
    }

    public void StartMoveCredits(GridEntity lookAtChild)
    {
        TextMeshProUGUI[] labels = GetComponentsInChildren<TextMeshProUGUI>();
        foreach(TextMeshProUGUI label in labels)
        {
            Color col = label.color;
            col.a = 1.0f;
            label.color = col;
        }
        Color c = finLabel.color;
        c.a = 0;
        finLabel.color = c;

        if(moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }
        creditsTransform.position = startPos;

        moveRoutine = CreditsRoutine(lookAtChild);
        StartCoroutine(moveRoutine);
    }

    private IEnumerator CreditsRoutine(GridEntity lookAtChild)
    {
        Vector2 delta = endPos / timeToMove;
        float elapsed = 0;

        Color c = finLabel.color;
        c.a = 0;
        finLabel.color = c;

        while(elapsed < timeToMove)
        {
            creditsTransform.anchoredPosition = startPos + elapsed * delta;

            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        }
        creditsTransform.anchoredPosition = endPos;

        float alphaDelta = 1.0f / fadeInFinTime;
        elapsed = 0;

        while(elapsed < fadeInFinTime)
        {
            c.a = alphaDelta * elapsed;
            finLabel.color = c;

            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        }
        c.a = 1.0f;
        finLabel.color = c;

      //  darknessController.SetChild(lookAtChild);
      //  darknessController.FadeTo(-10.0f, 1.0f/finalFadeDuration);
    } 

    public void SwapGrid()
    {
        foreach(Transform endImage in endImages)
        {
            endImage.gameObject.SetActive(true);
        }
        
        grid.gameObject.SetActive(false);
    }

     private void LateUpdate() 
    {
        if(hasStartedEnd)
        {
            if(elapsed < lastFrame.time)
            {
                camTransform.rotation = Quaternion.Euler(camRotation.Evaluate(elapsed),0,0);
                SetSunOffset(elapsed);
                elapsed += Time.deltaTime;
            }
            else
            {
                SetSunOffset(sunRotation.keys[sunRotation.keys.Length-1].time);
                camTransform.rotation = Quaternion.Euler(lastFrame.value,0,0);
                if(!hasStartedCredits)
                {
                    CreditsController.Instance.StartMoveCredits(lookAtEndChild);                    
                    hasStartedCredits = true;
                }
            }   
            return;
        }
    }
    public void StartCamRot(GridEntity lookAt)
    {
        lookAtEndChild = lookAt;
        hasStartedEnd = true;

        isAcceptingInput.Value = false;
    }

    private void SetSunOffset(float elapsed)
    {
        ShaderInstanceUtils.SetDarknessPosY(sunMaterial, sunRotation.Evaluate(elapsed));
    }
}
