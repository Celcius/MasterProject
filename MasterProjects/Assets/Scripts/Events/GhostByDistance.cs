using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    Color color;


    public override void OnRoomWillEnter() 
    {
        base.OnRoomWillEnter();
        color = rend.color;
    }

    private void FixedUpdate() 
    {
        float dist = Vector2.Distance(characterEntity.Value.transform.position, transform.position);
        float ratio = 1.0f - Mathf.Clamp01( dist / distToGhost);
        color.a = ghostAlpha.Evaluate(ratio);
        rend.color = color;
    }


}
