﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
    void Start()
    {
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

    
}
