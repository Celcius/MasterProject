using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CharacterRepresentation : MonoBehaviour
{
    [SerializeField]
    private Vector2Var facingDir;

    [SerializeField]
    private CharacterStateVar characterState;

    [SerializeField]
    private Sprite[] characterSpriteSheet;

    [SerializeField]
    private ParticleSystem cryingParticles;

    private SpriteRenderer spriteRenderer;

    private static Vector3 defaultDir = -Vector3.up;

    void Start()
    {
        cryingParticles.Stop();
        spriteRenderer = GetComponent<SpriteRenderer>();
        characterState.OnChange += OnStateChange;
        facingDir.OnChange += OnDirChange;
    }

    public void OnRespawn()
    {
        cryingParticles.Stop();
        cryingParticles.Clear();
    }

    private void OnStateChange(CharacterState oldVal, CharacterState newVal)
    {
        if(oldVal == newVal)
        {
            return;
        }

        UpdateState(facingDir.Value, newVal);
    }

    private void OnDirChange(Vector2 oldVal, Vector2 newVal)
    {
        if(oldVal == newVal)
        {
            return;
        }

        UpdateState(newVal, characterState.Value);
    }

    private void UpdateState(Vector2 dir, CharacterState state)
    {
        Vector3 chosenDir = (Vector3)dir + Vector3.forward;

        Sprite sprite = characterSpriteSheet[0];

        if(state != CharacterState.Crying)
        {
            cryingParticles.Stop();
        }
        
        switch(state)
        {        
            case CharacterState.Throwing:
            case CharacterState.Idle:
                chosenDir = defaultDir;
                sprite = characterSpriteSheet[0];
                break;

            case CharacterState.Walking:
                sprite = characterSpriteSheet[0];
                break;

            case CharacterState.Crying:
                 chosenDir = defaultDir;
                 sprite = characterSpriteSheet[2];
                 cryingParticles.Play();
                break;

            case CharacterState.Pushing:
                sprite = characterSpriteSheet[1];
                break;
        }

        transform.rotation = Quaternion.LookRotation(Vector3.forward, -chosenDir);
        spriteRenderer.sprite = sprite;
    }
}
