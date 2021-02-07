using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class GrandmaStateMoveToCryRandom : GrandmaState
{

    [SerializeField]
    private float moveSpeed = 10.0f;

    
    [SerializeField]
    private float stopDistance = 2.0f;

    [SerializeField]
    private TransformVar characterRepresentation;

    
    [SerializeField]
    private RandomSelectionTextBalloonString moveToCryStrings;

    [SerializeField]
    private RandomSelectionTextBalloonString cancelCryStrings;

    [SerializeField]
    private RandomSelectionTextBalloonString onReachCryStrings;


    [SerializeField]
    private RoomTileController roomController;

    private Vector3 nextRandomPos;
    private bool reached = false;
    public override bool CanGrab()
    {
        return false;
    }

    protected override void OnStateChange(CharacterState oldState, CharacterState newState) 
    {
        if(newState != CharacterState.Crying)
        {
            controller.SetState(GrandmaStateEnum.Idle);
        }
    }

    protected override void StartBehaviour()
    {
        reached = false;
        SetNextRandomPos();
        controller.Balloon.ShowText(moveToCryStrings.GetRandomSelection());
    }

    protected override void EndBehaviour()
    {
        controller.Balloon.ShowText(cancelCryStrings.GetRandomSelection());
    }

    private void Update() 
    {
        if(reached)
        {
            return;
        }

        float distance = Vector2.Distance(GranPos, nextRandomPos);
        if(distance < stopDistance || distance < moveSpeed*Time.deltaTime)
        {
            controller.transform.position = nextRandomPos;
            SetNextRandomPos();
            return;
        }
        
        Vector2 dir = (nextRandomPos - GranPos).normalized;
        controller.transform.position += (Vector3)dir * moveSpeed * Time.deltaTime;
    }

    private void SetNextRandomPos()
    {
        if(reached)
        {
            return;
        }

        Vector2Int characterPos = (Vector2Int)CameraMover.GridPosForWorldPos(characterRepresentation.Value.position);
        if(Vector2Int.Distance(characterPos, GranGridPos) <= 1.0f)
        {
            nextRandomPos = controller.transform.position;
            controller.Balloon.ShowText(onReachCryStrings.GetRandomSelection());
            reached = true;
            return;
        }

        Vector2Int[] neighbours = roomController.GetNeighbours(GranGridPos);
        if(neighbours == null || neighbours.Length == 0)
        {
            nextRandomPos = controller.transform.position;
            return;
        }

        List<Vector2Int> sorted = new List<Vector2Int>(neighbours);
        sorted.Remove(GranGridPos);

        sorted.Sort(new CompareDist(characterRepresentation.Value.position));
        nextRandomPos = CameraMover.WorldPosForGridPos((Vector3Int)sorted[0], 0);
    }

    private class CompareDist : IComparer<Vector2Int>
    {
        private Vector3 goalPos;
        public CompareDist(Vector3 goalPos)
        {
            this.goalPos = goalPos;
        }
        
        public int Compare(Vector2Int x, Vector2Int y)
        {
            return GetDist(x).CompareTo(GetDist(y));
        }

        private float GetDist(Vector2Int x)
        {
            return Vector2.Distance(CameraMover.WorldPosForGridPos((Vector3Int)x,0), goalPos);
        }
   }
}
