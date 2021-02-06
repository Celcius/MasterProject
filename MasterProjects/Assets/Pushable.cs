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

    private IEnumerator pushRoutine;
    Vector2Int pushMainDir;

    protected override void PlayerCollisionEnter(CharacterMovement character)
    {
        pushMainDir = SimplifyToDirAxis(character.MovingDir);

        if(pushMainDir.magnitude == 0)
        {
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
        Vector2Int newDir = pushMainDir = SimplifyToDirAxis(character.MovingDir);
        Debug.Log("" + character.MovingDir +" -> " +newDir);
        
        if(Mathf.Approximately(newDir.magnitude,0))
        {
            StopPush();
        } 
        else if(newDir != pushMainDir || pushRoutine == null)
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
    }

    private IEnumerator PushRoutine()
    {
        while(elapsed < pushTime)
        {
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        }
        PushObject();
    }

    private void PushObject()
    {
        Vector3Int goalPos = CameraMover.GridPosForWorldPos(transform.position) + (Vector3Int)pushMainDir;
        if(controller.IsEmptyPos((Vector2Int)goalPos))
        {
            transform.position =  CameraMover.WorldPosForGridPos(goalPos, transform.position.z);
        }
    }

    private Vector2Int SimplifyToDirAxis(Vector2 dir)
    {
        if(dir.x == dir.y)
        {
            return Vector2Int.zero;
        }

        return Mathf.Abs(dir.x) > Mathf.Abs(dir.y)? 
                            Mathf.RoundToInt(Mathf.Sign(dir.x)) * Vector2Int.right 
                            : Mathf.RoundToInt(Mathf.Sign(dir.y)) * Vector2Int.up;
    }

}
