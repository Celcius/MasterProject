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

    private RectTransform rectTransform;

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

    private void Start() 
    {
        rectTransform = GetComponent<RectTransform>();
        animationController = GetComponent<Animator>();
        animateText = GetComponent<AnimateTextOnStringVarChange>();

        if(stringVar == null)
        {
            return;
        }

        currentBalloonSpeed.OnChange += OnSpeedChange;

        stringVar.OnChange += OnStringChange;
        OnStringChange("", stringVar.Value);
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
            HideBalloon();
            return;
        }

        if(newVal.CompareTo(checkedString) == 0)
        {
            return;
        }

        ShowBalloon();
    }

    public void ShowText(TextBalloonString ballonString)
    {
        currentBalloonSpeed.Value = GameConstants.BallonSpeedFromEnum(ballonString.speedEnum);
        stringVar.Value = ballonString.textString;
    }

    public void ShowText(string textToShow, float textSpeed)
    {
        currentBalloonSpeed.Value = textSpeed;
        stringVar.Value = textToShow;
    }

    private void OnGUI() 
    {     
        if(checkedString.CompareTo(label.text) != 0)
        {
            StringChanged(checkedString, label.text);
        }
    }

    private void StringChanged(string oldVal, string newVal)
    {
        if(oldVal.CompareTo(newVal) == 0)
        {
            return;
        }

        float ratio = Mathf.Clamp01((float)(newVal.Length - minCharactersToIncrease)
                                 / (float)(charactersToMaxSize -minCharactersToIncrease));

        float height = newVal.Length > charactersToMaxSize? label.preferredHeight : minSize.y;

        Vector2 lerpedSize = new Vector2(Mathf.Lerp(minSize.x, maxSize.x, ratio),
                                         Mathf.Clamp(height, minSize.y, maxSize.y));
        rectTransform.sizeDelta = lerpedSize;

        checkedString = label.text;
    }

    private IEnumerator StopAfterDelay()
    {
        float timeToDelay = Mathf.Max(minTimeInScreen, remainTimePerLetter * checkedString.Length);
        yield return new WaitForSeconds(timeToDelay);
    
        HideBalloon();
    }

    public void HideBalloon(bool isInstant)
    {
        if(!isInstant)
        {
            HideBalloon();
        }
        else
        {
            animationController.SetBool("IsVisible", false);
            animationController.SetTrigger("Hide");
            checkedString = "";
        }
    }

    private void HideBalloon()
    {
        if(leaveRoutine != null)
        {
            StopCoroutine(leaveRoutine);
        }
        leaveRoutine = null;
        animationController.SetBool("IsVisible", false);
        checkedString = "";
    }

    public void BaloonLeft()
    {
        stringVar.Value = "";
    }

    private void ShowBalloon()
    {
        HideBalloon();
        animationController.SetBool("IsVisible", true);
        
        leaveRoutine = StopAfterDelay();
        StartCoroutine(leaveRoutine);
    }
}
