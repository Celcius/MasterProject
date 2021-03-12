using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathGrandmother : GrandmaController
{
    public override bool CanGrab => false;

    private bool hasPlayer = false;

    [SerializeField]
    private Vector3 quakePos;

    [SerializeField]
    private Vector3 characterGoalPos;

    private IEnumerator interactionRoutine;

    [SerializeField]
    private TextBalloonString[] preThrowDeathStrings;

    [SerializeField]
    private TextBalloonString[] grabThrowDeathStrings;

    [SerializeField]
    private TextBalloonString[] postThrowDeathStrings;

    [SerializeField]
    private float moveSpeed = 7;

    [SerializeField]
    private BoolVar isInputActive;

    [SerializeField]
    private TargetMover targetTransform;

    private bool isTargetOnPlace = false;

    [SerializeField]
    private float targetMoveSpeed = 2;

    [SerializeField]
    private IGrandmaController deadGrandma;

    [SerializeField]
    private float bigShakeIntensity;

    [SerializeField]
    private AnimationCurve bigShakeDamping;


    [SerializeField]
    private float bigShakeDuration;

    [SerializeField]
    private float smallShakeIntensity;
    
    [SerializeField]
    private AnimationCurve smallShakeDamping;

    [SerializeField]
    private float smallShakeDuration;

    [SerializeField]
    private Animator animator;

    private bool isDead = false;
    public override void ResetGrandma(bool isRespawn)
    {
        base.ResetGrandma(isRespawn);
        animator = GetComponent<Animator>();
        GrandmaStateIdle idle = (GrandmaStateIdle)GetStateForEnum(GrandmaStateEnum.Idle);
        idle.CanMoveToPlayer = false;
        
        targetTransform.transform.localPosition = Vector2.zero;
        isTargetOnPlace = false;
        
        if(interactionRoutine != null)
        {
            StopCoroutine(interactionRoutine);
        }
        interactionRoutine = DeathRoutine();
        
        StartCoroutine(interactionRoutine);
        
    }

    private void OnEnable() 
    {
        ResetGrandma(false);
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        CharacterMovement character = characterVar.Value.GetComponent<CharacterMovement>();
        GameCameraMover cam = ((GameCameraMover)CameraMover.Instance);
        cam.ShakeCamera(smallShakeIntensity, smallShakeDuration, smallShakeDamping, true);
        
        Vector2Int[] path = GetPath(quakePos, true);
        yield return SimpleWalkRoutine(path, moveSpeed, null);

        yield return ShowStringsSequentially(preThrowDeathStrings);
        
        do
        {
            path = GetPath(characterVar.Value.transform.position);
            yield return SimpleWalkRoutine(path, moveSpeed, null);
        }
        while(!isOnGrandmaVar.Value);

        isInputActive.Value = false;

        character.OnGrab(false);
        
        StartCoroutine(MoveTargetToPlace());

        yield return ShowStringsSequentially(grabThrowDeathStrings);

        while(!isTargetOnPlace)
        {
            yield return new WaitForEndOfFrame();
        }

        isTargetOnPlace = false;
        targetTransform.RemainActive = false;
        targetTransform.gameObject.SetActive(false);
        character.OnRelease(true);
        
        yield return new WaitForSeconds(2.0f);
        
        yield return ShowStringsSequentially(postThrowDeathStrings);
        Balloon.HideBalloon(false);
        yield return new WaitForSeconds(2.0f);

        cam.ShakeCamera(bigShakeIntensity, bigShakeDuration, bigShakeDamping, true);
        animator.enabled = true;
    }

    private IEnumerator ShowStringsSequentially(TextBalloonString[] strings)
    {
        for(int i = 0; i < strings.Length; i++)
        {
            TextBalloonString balloonStr = strings[i];
            Balloon.ShowText(balloonStr, false);
            yield return new WaitForSeconds(balloonStr.duration);
        }
    }

    private IEnumerator MoveTargetToPlace()
    {
        isTargetOnPlace = false;
        targetTransform.gameObject.SetActive(false);
        targetTransform.RemainActive = true;
        
        float dist = Vector2.Distance(characterGoalPos, targetTransform.transform.position);
        Vector2 dir = (characterGoalPos - targetTransform.transform.position).normalized;
        while(dist > targetMoveSpeed * Time.deltaTime)
        {
            yield return new WaitForEndOfFrame();

            targetTransform.transform.position += (Vector3)dir * targetMoveSpeed * Time.deltaTime;
            dist -= targetMoveSpeed*Time.deltaTime;

            representation.transform.right = (targetTransform.transform.position - transform.position).normalized;
        }
        targetTransform.transform.position = characterGoalPos;
        targetTransform.ManualUpdateTargetPosVar(characterGoalPos);

        isTargetOnPlace = true;
    }
    
    public override bool CheckLeaveRoom(Vector3 goalPos, System.Action callback)
    {
        if(gameObject.activeInHierarchy && !isDead)
        {
            return false;
        }
        callback?.Invoke();
        return false;
    }

    
    public override void OnBacktracking()
    {

    }

    public void OnAnimationEnded()
    {
        isDead = true;
        Balloon.HideBalloon(false);
    }

    public override bool IsGrandmaDead()
    {
        return isDead;
    }
}
 