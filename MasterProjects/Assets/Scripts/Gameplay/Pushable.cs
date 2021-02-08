using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class Pushable : PlayerCollideable
{
    [SerializeField]
    private float pushTime = 2.0f;
    private float elapsed = 0.0f;

    [SerializeField]
    private RoomTileController controller;

    [SerializeField]
    private PushableArrVar pushing;

    private IEnumerator pushRoutine;
    Vector2Int pushMainDir;

    protected override void PlayerCollisionEnter(CharacterMovement character)
    {
        pushMainDir = GeometryUtils.NormalizedMaxValueVector(character.MovingDir);

        if(pushMainDir.magnitude == 0 || !IsValidPushDir(character))
        {
            StopPush();
            return;
        }

        RestartPush();
    }

    protected override void PlayerCollisionExit(CharacterMovement character) 
    {
        StopPush();
    }

    protected override void PlayerCollisionStay(CharacterMovement character) 
    {
        Vector2Int newDir = pushMainDir = GeometryUtils.NormalizedMaxValueVector(character.MovingDir);
        
        if(Mathf.Approximately(newDir.magnitude,0) || !IsValidPushDir(character))
        {
            StopPush();
        } 

        else if(!Mathf.Approximately(newDir.magnitude,0) && (newDir != pushMainDir || pushRoutine == null))
        {
            pushMainDir = newDir;
            RestartPush();
        }
    }

    private void RestartPush()
    {
        elapsed = 0.0f;
        
        if(pushRoutine == null)
        {
            if(!pushing.Contains(this))
            {
                pushing.Add(this);
            }

            pushRoutine = PushRoutine();
            StartCoroutine(pushRoutine);
        }
    }

    private void StopPush()
    {
        if(pushRoutine != null)
        {
            StopCoroutine(pushRoutine);
        }
        pushRoutine = null;

        if(pushing.Contains(this))
        {
            pushing.Remove(this);
        }
    }

    private IEnumerator PushRoutine()
    {
        while(elapsed < pushTime)
        {
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        }
        PushObject();
        pushRoutine = null;
    }

    private void PushObject()
    {
        if(pushing.Contains(this))
        {
            pushing.Remove(this);
        }

        Vector3Int goalPos = CameraMover.GridPosForWorldPos(transform.position) + (Vector3Int)pushMainDir;
        if(controller.IsEmptyPos((Vector2Int)goalPos))
        {
            transform.position =  CameraMover.WorldPosForGridPos(goalPos, transform.position.z);
        }
    }

    private bool IsValidPushDir(CharacterMovement character)
    {
        Vector2Int gridPos = (Vector2Int)CameraMover.GridPosForWorldPos(transform.position);
        gridPos -= pushMainDir;
        Debug.DrawLine(transform.position + CameraMover.Instance.CellSize/2.0f, 
                       CameraMover.WorldPosForGridPos((Vector3Int)gridPos,0)+ CameraMover.Instance.CellSize/2.0f, 
                       Color.yellow);


        Debug.DrawLine(CameraMover.WorldPosForGridPos((Vector3Int)gridPos,0), 
                       CameraMover.WorldPosForGridPos((Vector3Int)character.GridPos, 0),
                       Color.magenta);
        return gridPos == character.GridPos;
    }

}
