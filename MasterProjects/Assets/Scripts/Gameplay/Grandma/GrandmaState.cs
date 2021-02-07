using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;
public enum GrandmaStateEnum
{
    Idle,
    MoveToCry,
    MoveToCall,
    Returning
}

public abstract class GrandmaState : MonoBehaviour
{

    [SerializeField]
    private GrandmaStateEnum enumState;
    public GrandmaStateEnum EnumState => enumState;

    protected CharacterStateVar characterState;
    protected GrandmaController controller;
    protected SpriteRenderer Representation => controller.representation;
    
    protected Vector2Int GranGridPos => (Vector2Int)controller.GridPos;
    protected Vector3 GranPos => controller.transform.position;

    protected Vector3 TargetPos => controller.GetTargetPos();

    protected virtual void Awake()
    {
        gameObject.SetActive(false);
    }

    public void OnStateStart(GrandmaController controller, CharacterStateVar characterState)
    {
        gameObject.SetActive(true);
        this.characterState = characterState;
        characterState.OnChange += OnStateChangeCallback;
        this.controller = controller;
        StartBehaviour();
    }

    public void OnStateEnd(GrandmaController controller)
    {
        characterState.OnChange -= OnStateChangeCallback; 
        EndBehaviour();
        gameObject.SetActive(false);
    }

    private void OnStateChangeCallback(CharacterState oldState, CharacterState newState)
    {
        if(oldState == newState)
        {
            return;
        }

        OnStateChange(oldState, newState);
    }

    protected abstract void StartBehaviour();
    protected abstract void EndBehaviour();
    protected abstract void OnStateChange(CharacterState oldState, CharacterState newState);

    public virtual bool CanGrab()
    {
        return true;
    }
}
