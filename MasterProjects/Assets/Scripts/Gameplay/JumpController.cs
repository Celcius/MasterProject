using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpController : MonoBehaviour
{
    [SerializeField]
    private float jumpDuration = 3.0f;

    [SerializeField]
    private AnimationCurve jumpScale;
    
    [SerializeField]
    private AnimationCurve jumpProgress;

    [SerializeField]
    private Transform targetTransform;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private string jumpLayer;

    [SerializeField]
    private int jumpSortOrder;

    [SerializeField]
    private float endDelay = 0;

    public void JumpTo(Vector3 pos, Action reachCallback, Action endCallback)
    {
        StartCoroutine(Jump(pos, reachCallback, endCallback));
    }

    private IEnumerator Jump(Vector3 pos, Action reachCallback, Action endCallback)
    {
        int startLayer = spriteRenderer.sortingLayerID;
        int startSortOrder = spriteRenderer.sortingOrder;
        spriteRenderer.sortingLayerName = jumpLayer;
        spriteRenderer.sortingOrder = jumpSortOrder;

        Vector3 startPos = targetTransform.position;
        Vector2 dir = ((Vector2)pos - (Vector2)targetTransform.position).normalized;
        float distance = Vector2.Distance(pos, targetTransform.position);
        Vector3 startScale = targetTransform.localScale;

        Debug.DrawLine(startPos,pos, Color.red, 5.0f);

        float elapsed = 0.0f;
        while (elapsed < jumpDuration)
        {
            yield return new WaitForEndOfFrame();
            
            elapsed += Time.deltaTime;
            float ratio = Mathf.Clamp01(elapsed / jumpDuration);
            targetTransform.position = startPos + (Vector3)dir * distance * jumpProgress.Evaluate(ratio);
            targetTransform.localScale = startScale * jumpScale.Evaluate(ratio);
        }
        targetTransform.localScale = startScale;
        targetTransform.position = pos;
        spriteRenderer.sortingLayerID = startLayer;
        spriteRenderer.sortingOrder = startSortOrder;
        reachCallback?.Invoke();

        if(endDelay > 0)
        {
            yield return new WaitForSeconds(endDelay);
        }
        endCallback?.Invoke();
    }
}
