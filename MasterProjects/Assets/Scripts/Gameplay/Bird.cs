using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : GridEntity
{
    [SerializeField]
    AnimationCurve birdMovement;

    [SerializeField]
    private Vector3 goalPos;

    private IEnumerator moveRoutine;
    
    private bool hasMoved = false;

    [SerializeField]
    private SoundHelperVar soundHelper;

    public override void OnRoomWillEnter() 
    {
        if(moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }
        base.OnRoomWillEnter();
        hasMoved = false;
        gameObject.SetActive(true);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) 
    {
        if(moveRoutine != null || hasMoved)
        {
            return;
        }

        if(other.tag == GameConstants.PLAYER_TAG)
        {
            soundHelper.Value.PlaySound(GameSoundTag.SFX_BIRD_WINGS);
            if(Random.Range(0,10) < 6)
            {
                GameSoundTag[] gullCries = {GameSoundTag.SFX_BIRD_GULL1, 
                                            GameSoundTag.SFX_BIRD_GULL2, 
                                            GameSoundTag.SFX_BIRD_GULL3,
                                            GameSoundTag.SFX_BIRD_GULL4};
                int index = Random.Range(0, gullCries.Length);
                soundHelper.Value.PlaySound(gullCries[index]);
            }
            hasMoved = true;
            moveRoutine = MoveRoutine();
            StartCoroutine(moveRoutine);
        }
    }


    private IEnumerator MoveRoutine()
    {
        if(birdMovement.keys.Length == 0)
        {
            yield break;
        }   

        Vector3 start = originalPosition;
        float lastMoment = birdMovement.keys[birdMovement.keys.Length-1].time;
        float elapsed = 0;
        while(elapsed < lastMoment)
        {
            transform.position = Vector3.Lerp(start, goalPos, birdMovement.Evaluate(elapsed));
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        }
        transform.position = goalPos;
        moveRoutine = null;
        gameObject.SetActive(false);
    }

}
