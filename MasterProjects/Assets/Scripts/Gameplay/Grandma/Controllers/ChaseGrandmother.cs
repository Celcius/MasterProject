using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class ChaseGrandmother : GrandmaController
{
    private struct ChaseCalculatedPos
    {
        public Vector3 pos;
        public float distToGrandma;
        public float distToChar;
    }

    public override bool CanGrab => false;

    [SerializeField]
    private TransformArrVar referencePositions;

    private Vector3 currentReferencePos;

    [SerializeField]
    private RandomSelectionTextBalloonString leaveString;

    [SerializeField]
    private TextBalloonString startText;
    [SerializeField]
    private TextBalloonString reachStartText;

    [SerializeField]
    private TextBalloonString[] finishTexts;

    [SerializeField]
    RandomSelectionTextBalloonString moveStrings;
    
    [SerializeField]
    private float finishTextDuration = 2.0f;

    [SerializeField]
    private float moveSpeed = 7.5f;
    [SerializeField]
    private float startMoveSpeed = 7.5f;

    private float minSpeedDistance = 5.0f;

    [SerializeField]
    private GrandmaController grandmaPrefab;

    private bool hasStarted = false;

    private bool movingToPos = false;

    private IEnumerator moveRoutine = null;
    
    public override void ResetGrandma(bool isRespawn)
    {
        base.ResetGrandma(isRespawn);

        hasStarted = false;
        movingToPos = false;
        currentReferencePos = referencePositions.Value[0].position;
        GrandmaStateIdle idle = (GrandmaStateIdle)GetStateForEnum(GrandmaStateEnum.Idle);
        idle.CanMoveToPlayer = false;
        StartCoroutine(StartRoutine());

        for(int i = 0; i < referencePositions.Count(); i++)
        {
            int j = (i+1) % referencePositions.Count();
            Debug.DrawLine(referencePositions.Value[i].transform.position, referencePositions.Value[j].transform.position, Color.red, 5.0f);
        }
    }

    private void OnEnable() 
    {
        ResetGrandma(false);
    }

    private IEnumerator StartRoutine()
    {
        yield return new WaitForEndOfFrame();
        Balloon.ShowText(startText, false);
        
        Vector2Int[] path = GetPath(referencePositions.Value[0].position, true);
        MoveTo(path, ()=> 
        {
            Balloon.ShowText(reachStartText, false, false);
            hasStarted = true;
        }, 
        startMoveSpeed);
    }

    private void LateUpdate() 
    {
        if(!hasStarted || movingToPos)
        {
            return;
        }

        float dist = Vector2.Distance(characterVar.Value.transform.position, transform.position); 
        
        if(dist <= minSpeedDistance)
        {
            Vector3 goalPos = GetNextGoalPos();

            System.Action moveCallback =  ()=> 
            { 
                movingToPos = false; 
            };

            
            Vector2Int[] path = GetPath(goalPos, true);
            movingToPos = true;
            MoveTo(path, moveCallback, moveSpeed);

            TextBalloonString balloonString = moveStrings.GetRandomSelection();
            if(!string.IsNullOrEmpty(balloonString.textString))
            {
                Balloon.ShowText(balloonString);
            }
                
        }
    }

    private Vector3 GetNextGoalPos()
    {      
        List<ChaseCalculatedPos> calculatedList = new List<ChaseCalculatedPos>();
        foreach(Transform posTrans in referencePositions.Value)
        {
            Vector3 pos = posTrans.position;
            if(pos == currentReferencePos)
            {
                continue;
            }

            ChaseCalculatedPos calculatedPos = new ChaseCalculatedPos();
            calculatedPos.pos = pos;
            calculatedPos.distToChar = Vector3.Distance(characterVar.Value.transform.position, pos);
            calculatedPos.distToGrandma = Vector3.Distance(transform.position, pos);

            calculatedList.Add(calculatedPos);
        }

        calculatedList.Sort(ClosestToGrandma);
        calculatedList.RemoveRange(2,calculatedList.Count-2);
        calculatedList.Sort(FurthestToChild);

        Vector3 chosenPos = calculatedList[0].pos;
        currentReferencePos = chosenPos;
        return GetUnoccupiedAdjacentPos(chosenPos);
    }

    public Vector3 GetUnoccupiedAdjacentPos(Vector3 pos)
    {
        Vector2Int gridPos = (Vector2Int)CameraMover.GridPosForWorldPos(pos);
        List<Vector3> unoccupiedPositions = new List<Vector3>();
        float refDist = Vector2.Distance(pos, characterVar.Value.transform.position);
        
        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                Vector2Int curPos = gridPos + new Vector2Int(i,j);
                Vector3 worldCurPos = CameraMover.WorldPosForGridPos((Vector3Int) curPos,0);

                if(Vector2.Distance(worldCurPos, characterVar.Value.transform.position) < refDist)
                {
                    continue; // no closer to player;
                }
                
                GridEntity[] entities = GridRegistry.Instance.GetEntitiesAtPos((Vector3Int)curPos);
                if(entities != null)
                {
                    foreach(GridEntity entity in entities)
                    {
                        if(entity.transform != transform && entity.transform != characterVar.Value.transform)
                        {
                            continue; // Don't add
                        }
                    }
                }
                unoccupiedPositions.Add(worldCurPos);
            }
        }

        if(unoccupiedPositions.Count == 0)
        {
            return transform.position;
        }
        
        return unoccupiedPositions[Random.Range(0, unoccupiedPositions.Count)];
    }

    private static int ClosestToGrandma(ChaseCalculatedPos x, ChaseCalculatedPos y)
    {
        return x.distToGrandma.CompareTo(y.distToGrandma);
    }

    private static int FurthestToChild(ChaseCalculatedPos x, ChaseCalculatedPos y)
    {
        return y.distToChar.CompareTo(x.distToChar);
    }

    public override void CheckLeaveRoom(Vector3 goalPos, System.Action callback)
    {
        if(hasStarted)
        {
            Balloon.ShowText(leaveString.GetRandomSelection());
        }
        
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if(!hasStarted)
        {
            return;
        }

        if(other.tag == GameConstants.PLAYER_TAG)
        {
            if(moveRoutine != null)
            {
                StopCoroutine(moveRoutine);
            }
            hasStarted = false;
            StartCoroutine(EndRoutine());
        }
    }

    private IEnumerator EndRoutine()
    {
        Balloon.HideBalloon(true);
        for(int i = 0; i < finishTexts.Length; i++)
        {
            Balloon.ShowText(finishTexts[i]);
            yield return new WaitForSeconds(finishTextDuration);
        }
        
        GrandmaController grandma = Instantiate<GrandmaController>(grandmaPrefab, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }


    private void MoveTo(Vector2Int[] path, System.Action callback, float speed)
    {
        if(moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }
        moveRoutine = SimpleWalkRoutine(path, speed, callback);
        StartCoroutine(moveRoutine);
    }

}
 