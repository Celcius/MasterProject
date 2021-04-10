using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AmoaebaUtils;

public class FadeLabelAndImagesFromString : FadeLabelFromStringVar
{
    [SerializeField]
    private Image[] images;

    protected override IEnumerator FadeTo(float goal)
    {
        float delta = (goal - label.alpha) / fadeInTime;
        float elapsed = 0;
        float alpha = label.alpha;
        Color c = label.color;
        while(elapsed <= fadeInTime)
        {
            yield return new WaitForEndOfFrame();
            alpha += Time.deltaTime * delta;
            elapsed += Time.deltaTime;
            c.a = alpha;
            label.color = c;
            foreach(Image image in images)
            {
                Color iColor = image.color;
                iColor.a = c.a;
                image.color = iColor;
            }
        }
        c.a = goal;
    }
}
