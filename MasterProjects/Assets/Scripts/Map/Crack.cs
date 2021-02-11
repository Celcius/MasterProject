﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;
using Sirenix.OdinInspector;

public class Crack : PlayerCollideable
{
    [SerializeField, OnValueChanged("OnEnable")]
    private bool startAsHole;

    [SerializeField]
    private GameObject holeRepresentation;
    
    [SerializeField]
    private GameObject crackRepresentation;

    [SerializeField, Range(0,0.5f), GUIColor(0,1.0f,0.0f,1.0f)]
    private float insideRadius;

    [SerializeField]
    private AnimationCurve holePullIntensity;

    [SerializeField]
    private float timeToFall = 2.0f;

    private float timeSinceStart = 0;

    private bool isHole = false;

    private Rigidbody2D characterBody;

    private Collider2D holeCollider;

    [SerializeField]
    private BoolVar isAcceptingInput;

    [SerializeField]
    private float timeInHole = 1.0f;
    private bool isFalling = false;

    private static Crack currentlyPulling = null;

    private void OnEnable() 
    {
        holeRepresentation.gameObject.SetActive(startAsHole);
        crackRepresentation.gameObject.SetActive(!startAsHole);
        isHole = startAsHole;
        holeCollider = GetComponent<Collider2D>();
    }

    protected override void PlayerTriggerEnter(CharacterMovement character)
    {
        timeSinceStart = 0;
        characterBody = character.GetComponent<Rigidbody2D>();
    }

    protected override void PlayerTriggerExit(CharacterMovement character) 
    {
        if(isHole)
        {
            return;
        }

        timeSinceStart = 0;
        holeRepresentation.gameObject.SetActive(true);
        crackRepresentation.gameObject.SetActive(false);        
        isHole = true;
    }

    protected override void PlayerTriggerStay(CharacterMovement character)
    {
        if(!isHole || isFalling)
        {
            return;
        }

        
        timeSinceStart += Time.deltaTime;

        CheckCurrentlyPulling(character);

        if(currentlyPulling != this)
        {
            return;
        }


        Vector3 forceDir = ((Vector2)holeCollider.bounds.center 
                            - (Vector2)character.ColliderBounds.center).normalized;
        characterBody.transform.position = character.transform.position + forceDir * GetHolePullIntensity() *  Time.deltaTime;

        if(timeSinceStart >= timeToFall ||  
           GeometryUtils.IsCircleCollision(transform.position, 
                                           insideRadius, 
                                           character.ColliderBounds.center, 
                                           character.ColliderBounds.extents.x))
        {   
            isFalling = true;
            StartCoroutine(FallInHole(character));
        }

    }

    private void CheckCurrentlyPulling(CharacterMovement character)
    {
        if(currentlyPulling == null)
        {
            currentlyPulling = this;
            return;
        }

        float pullingDist = Vector2.Distance(currentlyPulling.holeCollider.bounds.center, character.ColliderBounds.center);
        float thisDist = Vector2.Distance(holeCollider.bounds.center, character.ColliderBounds.center);
        if(thisDist < pullingDist)
        {
            currentlyPulling = this;
        }
    }   

    protected override void OnTriggerEnter2D(Collider2D other) 
    {
        Boulder isBoulder = other.GetComponent<Boulder>();
        if(isBoulder)
        {
            isBoulder.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }
        else
        {
            base.OnTriggerEnter2D(other);
        }
    }

    private IEnumerator FallInHole(CharacterMovement character)
    {
        isAcceptingInput.Value = false;
        float maxIntensity = holePullIntensity.Evaluate(1.0f);
        while(Vector2.Distance(character.ColliderBounds.center, holeCollider.bounds.center) >= maxIntensity *  Time.deltaTime)
        {
               Vector3 forceDir = ((Vector2)holeCollider.bounds.center 
                                   - (Vector2)character.ColliderBounds.center).normalized;
               characterBody.transform.position = character.transform.position + forceDir * maxIntensity *  Time.deltaTime;
               yield return  new WaitForEndOfFrame();
        }
        character.Hide();
        yield return new WaitForSeconds(timeInHole);

        character.Show();
        isAcceptingInput.Value = true;
        characterBody.transform.position = CameraMover.WorldPosForGridPos((Vector3Int)character.PrevSafePos, 0);
        timeSinceStart = 0;
        isFalling = false;
    }

    private float GetHolePullIntensity()
    {
        return holePullIntensity.Evaluate(Mathf.Clamp01(timeSinceStart/timeToFall));
    }

    private void OnDrawGizmos() 
    {
        Color oldColor = Gizmos.color;

        Vector2[] innerCirclePoints = GeometryUtils.PointsInCircle(insideRadius, 20);
        DrawCircle(innerCirclePoints, Color.green);

        Gizmos.color = oldColor;
    }

    private void DrawCircle(Vector2[] points, Color color)
    {
        Gizmos.color = color;

        for(int i = 0; i < points.Length; i++)
        {
            int j = (i+1) % points.Length;
            Gizmos.DrawLine(points[i], points[j]);
        }
    }
}