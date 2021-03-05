using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AmoaebaUtils;

public class TextBalloon : MonoBehaviour
{
    [SerializeField]
    private Vector2 minSize;
    
    [SerializeField]
    private Vector2 maxSize;

    [SerializeField]
    private int charactersToMaxSize;
    [SerializeField]
    private int minCharactersToIncrease;

    [SerializeField]
    private TextMeshProUGUI label;

    protected RectTransform rectTransform;

    [SerializeField]
    private float remainTimePerLetter;


    [SerializeField]
    private float minTimeInScreen;

    private Animator animationController;

    private string checkedString = "";

    private IEnumerator leaveRoutine = null;

    private AnimateTextOnStringVarChange animateText;

    
    [SerializeField]
    private StringVar stringVar;

    [SerializeField]
    private FloatVar currentBalloonSpeed;

    private bool isShowing;
    public bool IsShowing => isShowing;
    
    public bool hiding = false;
    public bool IsLeavingOrHidden => !IsShowing || hiding;

    public string Text => checkedString;
    private bool hasComponents = false;

    public delegate void OnHide();
    public event OnHide OnHideCallback;
    public event OnHide OnWillHideCallback;

    private bool canInterruptOngoing = true;

    protected virtual void Start() 
    {
        GetComponents();
        isShowing = false;

        if(stringVar == null)
        {
            return;
        }

        currentBalloonSpeed.OnChange += OnSpeedChange;

        stringVar.OnChange += OnStringChange;
        OnStringChange("", stringVar.Value);
    }

    private void GetComponents()
    {
        rectTransform = GetComponent<RectTransform>();
        animationController = GetComponent<Animator>();
        animateText = GetComponent<AnimateTextOnStringVarChange>();
        hasComponents = true;
    }

    private void OnDestroy() 
    {
        currentBalloonSpeed.OnChange -= OnSpeedChange;
        stringVar.OnChange -= OnStringChange;    
    }

    private void OnSpeedChange(float oldVal, float newVal)
    {
        animateText.TimePerLetter = newVal;
    }

    private void OnStringChange(string oldVal, string newVal)
    {
        
        if(string.IsNullOrWhiteSpace(newVal))
        {
            ForceHideBalloon();
            return;
        }

        if(newVal.CompareTo(checkedString) == 0)
        {
            return;
        }

        checkedString = newVal;

        ShowBalloon();
    }

    public void ShowText(TextBalloonString ballonString, bool instant = false, bool canInterrupt = true)
    {
        ShowText(ballonString.textString, GameConstants.BallonSpeedFromEnum(ballonString.speedEnum), instant, canInterrupt);
    }

    public void ShowText(string textToShow, float textSpeed, bool instant = false, bool canInterrupt = true)
    {
        if(!this.canInterruptOngoing)
        {
            return;
        }
        
        hiding = false;
        GetComponents();
        if(!instant)
        {
            UpdateRectSize(minSize);
            isShowing = true;
            currentBalloonSpeed.Value = textSpeed;
            stringVar.Value = textToShow;
            this.canInterruptOngoing = canInterrupt;
        }
        else
        {
            StringChanged("", textToShow);
            this.canInterruptOngoing = canInterrupt;
        }
    }

    protected virtual void OnGUI() 
    {     
        if(checkedString.CompareTo(label.text) != 0)
        {
            StringChanged(checkedString, label.text);
        }
    }

    protected virtual void StringChanged(string oldVal, string newVal)
    {
        if(oldVal.CompareTo(newVal) == 0)
        {
            return;
        }
        if(!hasComponents)
        {
            GetComponents();
        }

        float ratio = Mathf.Clamp01((float)(newVal.Length - minCharactersToIncrease)
                                 / (float)(charactersToMaxSize -minCharactersToIncrease));

        float height = newVal.Length > charactersToMaxSize? label.preferredHeight : minSize.y;
        
        Vector2 lerpedSize = new Vector2(Mathf.Lerp(minSize.x, maxSize.x, ratio),
                                         Mathf.Clamp(height, minSize.y, maxSize.y));

        UpdateRectSize(lerpedSize);

        checkedString = label.text;
    }

    private IEnumerator StopAfterDelay(int len)
    {
        float timeToDelay = Mathf.Max(minTimeInScreen, remainTimePerLetter * len);
        yield return new WaitForSeconds(timeToDelay);
    
        ForceHideBalloon();
    }

    public void HideBalloon(bool isInstant)
    {
        if(!isInstant)
        {
            ForceHideBalloon();
        }
        else
        {
            GetComponents();
            animationController.SetBool("IsVisible", false);
            animationController.SetTrigger("Hide");
            checkedString = "";
            stringVar.Value = "";
            isShowing = false;
            hiding = false;
            canInterruptOngoing = true;
            OnHideCallback?.Invoke();
        }
    }

    private void ForceHideBalloon(bool callback = true)
    {
        if(leaveRoutine != null)
        {
            StopCoroutine(leaveRoutine);
        }

        leaveRoutine = null;
        animationController.SetBool("IsVisible", false);
        checkedString = "";
        canInterruptOngoing = true;
        hiding = true;
        if(callback)
        {
            OnWillHideCallback?.Invoke();
        }
    }

    public void BaloonLeft()
    {
        hiding = false;
        isShowing = false;
        stringVar.Value = "";
        OnHideCallback?.Invoke();
        canInterruptOngoing = true;
    }

    private void ShowBalloon()
    {
        int stringlen = checkedString.Length;
        ForceHideBalloon(false);
        hiding = false;
        isShowing = true;
        animationController.SetBool("IsVisible", true);
        
        leaveRoutine = StopAfterDelay(stringlen);
        StartCoroutine(leaveRoutine);
    }

    protected virtual void UpdateRectSize(Vector2 size)
    {
        rectTransform.sizeDelta = size;
    }
}
