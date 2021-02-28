using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndChild : MonoBehaviour
{
    [SerializeField]
    private Vector3 endPos;

    [SerializeField]
    private float moveSpeed = 3.0f;

    private Animator animator;

    private IEnumerator moveRoutine = null;

    private void OnEnable() 
    {
        animator = GetComponent<Animator>();
        if(moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }
        moveRoutine = MoveToEnd();
        StartCoroutine(moveRoutine);
    }

    private IEnumerator MoveToEnd()
    {
        float dist = Vector2.Distance(endPos, transform.position);
        Vector2 dir = (endPos - transform.position).normalized;
        while(dist > moveSpeed * Time.deltaTime)
        {
            yield return new WaitForEndOfFrame();

            transform.position += (Vector3)dir * moveSpeed * Time.deltaTime;
            dist -= moveSpeed*Time.deltaTime;
        }
        transform.position = endPos;
        animator.enabled = true;
    }
}
