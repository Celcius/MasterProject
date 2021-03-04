using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGrandmother2 : GrandmaController
{
    [SerializeField]
    private TextBalloonString[] onGrabTexts;

    [SerializeField]
    private TextBalloonString[] onReleaseTexts;


    [SerializeField]
    private TextBalloonString[] cancelThrowTexts;

    [SerializeField]
    private TextBalloonString leaveText;

    [SerializeField]
    private TextBalloonString cutText;

    [SerializeField]
    private TextBalloonString[] againText;

    [SerializeField]
    private RandomSelectionTextBalloonString backtrackEyesClosed;

    [SerializeField]
    private int timesToThrow = 2;

    private int timesThrown = 0;

    private IEnumerator programmedRoutine;

    [SerializeField]
    private Vector3[] goalPos;

    [SerializeField]
    private Vector2Int[] destroyTiles;

    [SerializeField]
    private Transform cutPrefab;

    [SerializeField]
    private StringVar[] tutorialStringVars;

    [SerializeField]
    private string[] tutorialStrings;

    [SerializeField]
    private BoolVar canCall;

    private bool canLeave = false;
    protected override void Start() 
    {
        base.Start();
        ResetGrandma(false);
    }
    public override void ResetGrandma(bool isRespawn)
    {
        if(programmedRoutine != null)
        {
            StopCoroutine(programmedRoutine);
            programmedRoutine = null;
        }

        base.ResetGrandma(isRespawn);
        timesThrown = 0;
        canLeave = false;
        ShowTutString(0);
        canCall.Value = true;
    }


    [SerializeField]
    public override void ReleaseCharacter(CharacterMovement character, bool throwChar)
    {
        base.ReleaseCharacter(character, throwChar);
        if(throwChar)
        {
            ShowBalloonStringByInteraction(onReleaseTexts);
            timesThrown++;
            if(timesThrown >= timesToThrow)
            {
                OnTutorialComplete();
            }
            else 
            {
                if(programmedRoutine != null)
                {
                    StopCoroutine(programmedRoutine);
                }

                programmedRoutine = TalkRoutine();
                StartCoroutine(programmedRoutine);
            }
        }
        else
        {
            ShowBalloonStringByInteraction(cancelThrowTexts);
        }
    }

    [SerializeField]
    public override bool GrabCharacter(CharacterMovement character)
    {
        
        ShowTutString(1);

        bool didGrab = base.GrabCharacter(character);
        if(timesThrown < timesToThrow)
        {
            ShowBalloonStringByInteraction(onGrabTexts);
        }
        return didGrab;
    }

    public override void OnBacktracking()
    {
        if(timesThrown >= timesToThrow)
        {
            base.OnBacktracking();
        }
        else
        {
            if(Balloon.IsLeavingOrHidden)
            {
                Balloon.ShowText(backtrackEyesClosed.GetRandomSelection());
            }
        }
    }

    public override bool CheckLeaveRoom(Vector3 goalPos, System.Action callback)
    {
        if(canLeave)
        {
            callback.Invoke();
            return false;
        }
        if(timesThrown >= timesToThrow)
        {
            return base.CheckLeaveRoom(goalPos, callback);
        }
        else
        {
            if(Balloon.IsLeavingOrHidden)
            {
                Balloon.ShowText(backtrackEyesClosed.GetRandomSelection());
            }
        }
        return false;
    }
        
    private void ShowBalloonStringByInteraction(TextBalloonString[] strings)
    {
        int index = Mathf.Clamp(timesThrown, 0, strings.Length-1);
        Balloon.ShowText(strings[index]);
    }

    private void OnTutorialComplete()
    {
        HideTutStrings();
        
        GrandmaStateIdle idle = (GrandmaStateIdle)GetStateForEnum(GrandmaStateEnum.Idle);
        idle.CanMoveToPlayer = false;
        if(programmedRoutine != null)
        {
            StopCoroutine(programmedRoutine);
        }
        programmedRoutine = LeaveRoutine();
        StartCoroutine(programmedRoutine);
    }

    private IEnumerator LeaveRoutine()
    {
        yield return new WaitForSeconds(3);
        Balloon.ShowText(leaveText);
        yield return MoveToGoal(0, 4);
        
        yield return new WaitForSeconds(1);
        
        Cut();

        yield return new WaitForSeconds(0.5f);
        Balloon.HideBalloon(false);
        yield return MoveToGoal(1, 4);
        programmedRoutine = null;
    }

    private IEnumerator TalkRoutine()
    {
        yield return new WaitForSeconds(1.1f);
        int againIndex = timesThrown < timesToThrow-1? 0 : 1;
        Balloon.ShowText(againText[againIndex]);
        ShowTutString(0);
    }

    private IEnumerator MoveToGoal(int index, float speed)
    {
        Vector3 startPos = transform.position;
        Vector3 pos = goalPos[index];
        
        float time = Vector2.Distance(startPos, pos) / speed;
        Vector3 delta = (pos - startPos)/time;
        while(time > 0)
        {
            yield return new WaitForEndOfFrame();
            time-= Time.deltaTime;
            transform.position += delta * Time.deltaTime;
        }
        transform.position = pos;
    }

    private void Cut()
    {
        Balloon.ShowText(cutText);
        Transform cut = Instantiate<Transform>(cutPrefab, 
                                        RepresentationPos,
                                        Quaternion.identity);
        cut.right = Vector2.right;

        foreach(Vector2Int pos in destroyTiles)
        {
            GridEntity[] entities = GridRegistry.Instance.GetEntitiesAtPos((Vector3Int)pos);
            if(entities == null)
            {
                continue;    
            }

            for(int i = entities.Length-1; i >= 0; i--)
            {
                GridEntity entity = entities[i];
                Cuteable cuteable = entity.GetComponent<Cuteable>();
                if(cuteable != null)
                {
                    cuteable.Cut();
                }
            }
        }

        canLeave = true;
    }

    private void ShowTutString(int index)
    {
        index = Mathf.Clamp(index, 0, tutorialStringVars.Length-1);
        int nextIndex = (index+1) % tutorialStringVars.Length;
        tutorialStringVars[nextIndex].Value = "";
        tutorialStringVars[index].Value = tutorialStrings[index];
    }

    private void HideTutStrings()
    {
        foreach(StringVar tutString in tutorialStringVars)
        {
            tutString.Value = "";
        }
    }
}
 