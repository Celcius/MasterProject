using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GhostByDistance : GridEntity
{
    [SerializeField]
    private float distToGhost;

    [SerializeField]
    private AnimationCurve ghostAlpha;

    [SerializeField]
    private GridEntityVar characterEntity;

    [SerializeField]
    private SpriteRenderer rend;

    [SerializeField]
    private SoundHelperVar soundHelper;
    Color color;

    [SerializeField]
    private float musicRatio = 0.75f;

    [SerializeField]
    private AudioMixerSnapshot remembranceSnapshot;

    [SerializeField]
    private AudioMixerSnapshot defaultSnapshot;

    public override void OnRoomWillEnter() 
    {
        base.OnRoomWillEnter();
        color = rend.color;
    }

    public override void OnRoomEnter() 
    {
        remembranceSnapshot.TransitionTo(0);
    }

    public override void OnRoomWillLeave()  
    {
        defaultSnapshot.TransitionTo(0);
    }

    private void FixedUpdate() 
    {
        float dist = Vector2.Distance(characterEntity.Value.transform.position, transform.position);
        float ratio = 1.0f - Mathf.Clamp01( dist / distToGhost);
        if(ratio >= musicRatio)
        {
            soundHelper.Value.MusicPlayer.PlayStyle(MusicPlayer.MusicStyle.Remembrance);
        }
        color.a = ghostAlpha.Evaluate(ratio);
        rend.color = color;
    }


}
