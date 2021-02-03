using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMover : MonoBehaviour
{
    [SerializeField]
    private IInputController input;
    [SerializeField]
    private CharacterStateVar stateVar;

    [SerializeField]
    private float moveSpeed = 5.0f;

    [SerializeField]
    private Vector2Var targetPosVar;

    [SerializeField]
    private GrandmaScriptVar grandmaVar;

    private void Start() 
    {
        stateVar.OnChange += OnStateChange;
        OnStateChange(CharacterState.Idle, stateVar.Value);
    }

    private void OnDestroy() 
    {
        stateVar.OnChange -= OnStateChange;
    }

    private void OnStateChange(CharacterState oldVal, CharacterState newVal)
    {
        gameObject.SetActive(newVal == CharacterState.Throwing);
        
        Vector2 resetPos = (Vector2)grandmaVar.Value.transform.position;
        transform.position = (Vector3) resetPos + Vector3.up * transform.position.z;
        targetPosVar.Value = transform.position;
    }

    private void Update()
    {
        Vector3 dir = input.GetMovementAxis();
        if(!Mathf.Approximately(dir.magnitude, 0))
        {
            transform.position = transform.position + moveSpeed*dir * Time.deltaTime;
            targetPosVar.Value  = transform.position;
        }
    }
}
