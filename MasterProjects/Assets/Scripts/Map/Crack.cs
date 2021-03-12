using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;
using Sirenix.OdinInspector;

[RequireComponent(typeof(GridEntity))]
public class Crack : PlayerCollideable
{
    [SerializeField, OnValueChanged("OnEnable")]
    private bool startAsHole;

    [SerializeField]
    private GameObject holeRepresentation;
    
    [SerializeField]
    private GameObject crackRepresentation;

    [SerializeField]
    private RoomTileController roomTileController;

    [SerializeField, Range(0,0.5f), GUIColor(0,1.0f,0.0f,1.0f)]
    private float insideRadius;

    [SerializeField]
    private AnimationCurve holePullIntensity;

    [SerializeField]
    private float timeToFall = 2.0f;

    private float timeSinceStart = 0;

    private bool isHole = false;
    public bool IsHole => isHole;

    private Rigidbody2D characterBody;

    private Collider2D holeCollider;

    [SerializeField]
    private BoolVar isAcceptingInput;

    [SerializeField]
    private float timeInHole = 1.0f;
    private bool isFalling = false;

    private static Crack currentlyPulling = null;

    private bool started = false;

    [SerializeField]
    private SoundHelperVar soundHelperVar;

    private void OnEnable() 
    {
        roomTileController.OnRoomProcessed += UpdateHoleState;
        SetHoleState(startAsHole);
        holeCollider = GetComponent<Collider2D>();
    }

    private void OnDisable() 
    {
        roomTileController.OnRoomProcessed -= UpdateHoleState;    
    }

    private void UpdateHoleState()
    {
        SetHoleState(isHole);
    }

    protected override void EntityTriggerEnter(CharacterMovement character)
    {
        timeSinceStart = 0;
        characterBody = character.GetComponent<Rigidbody2D>();
    }

    protected override void EntityTriggerExit(CharacterMovement character) 
    {
        if(isHole)
        {
            return;
        }
        
        soundHelperVar.Value.PlaySound(GameSoundTag.SFX_CRACK_CRUMBLE);
        SetHoleState(true);
        
        timeSinceStart = 0;

    }

    private void SetHoleState(bool isHole)
    {
        holeRepresentation.gameObject.SetActive(isHole);
        crackRepresentation.gameObject.SetActive(!isHole);    
        
        this.isHole = isHole;    
    }

    private void LateUpdate() 
    {
        if(!started)
        {
            SetHoleState(isHole);
            started = true;
        }
    }

    protected override void EntityTriggerStay(CharacterMovement character)
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

    protected override void OnTriggerExit2D(Collider2D other) 
    {
        if(other.tag == GameConstants.GRANDMOTHER_TAG)
        {
            if(!isHole)
            {
                soundHelperVar.Value.PlaySound(GameSoundTag.SFX_CRACK_CRUMBLE);
                SetHoleState(true);
            }
        }
        else
        {
            base.OnTriggerExit2D(other);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other) 
    {
        if(Time.time <= 1.0f)
        {
            return;
        }
        Boulder isBoulder = other.GetComponent<Boulder>();
        if(isBoulder)
        {
            isBoulder.gameObject.SetActive(false);
            // Uncomment for only hole is filled behaviour
            //if(isHole)
            //{
            soundHelperVar.Value.StopSound(GameSoundTag.SFX_PUSH_STONE);
            soundHelperVar.Value.PlaySound(GameSoundTag.SFX_CRACK_FILL);
            this.gameObject.SetActive(false);
            /*}
            else
            {
                soundHelperVar.Value.PlaySound(GameSoundTag.SFX_CRACK_CRUMBLE);
                SetHoleState(true);
            }*/
            
            Vector2Int gridPos = (Vector2Int)CameraMover.GridPosForWorldPos(holeRepresentation.transform.position);
        }
        else
        {
            base.OnTriggerEnter2D(other);
        }
    }

    private IEnumerator FallInHole(CharacterMovement character)
    {
        soundHelperVar.Value.PlaySound(GameSoundTag.SFX_FALL_HOLE);

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
