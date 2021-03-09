using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraMover : CameraMover
{
    CharacterMovement movement;

    [SerializeField]
    private SoundHelperVar soundHelper;

    protected override void Start() 
    {
        base.Start();
        lookAtGridEntity.OnChange += OnCharChange;
        OnCharChange(null, lookAtGridEntity.Value);
    }

    protected override void OnDestroy() 
    {
        base.OnDestroy();
        lookAtGridEntity.OnChange -= OnCharChange;
    }

    private void OnCharChange(GridEntity oldVal, GridEntity newVal)
    {
        movement = newVal == null? null : newVal.GetComponent<CharacterMovement>();
    }

    public override bool CanMove(Vector2Int newPlayerPos)
    {
        return movement == null || !movement.IsGrabbed;
    }

    protected override IEnumerator CameraShake(float time, float magnitude, float damping)
    {
        Vector3 initialPosition = GetTargetCameraPosition();
        float duration = time;
        while(duration >= 0)
        {
            if(magnitude >= 0.8f)
            {
                soundHelper.Value.StopSound(GameSoundTag.SFX_QUAKE);
                if(!soundHelper.Value.IsPlaying(GameSoundTag.SFX_LARGE_QUAKE))
                {
                    soundHelper.Value.PlaySound(GameSoundTag.SFX_LARGE_QUAKE,false);
                }    
            } 
            else 
            {
                soundHelper.Value.StopSound(GameSoundTag.SFX_LARGE_QUAKE);
                if(!soundHelper.Value.IsPlaying(GameSoundTag.SFX_QUAKE))
                {
                    soundHelper.Value.PlaySound(GameSoundTag.SFX_QUAKE, true);
                }
            }
           
            transform.localPosition = initialPosition + UnityEngine.Random.insideUnitSphere * magnitude;
        
            duration -= Time.deltaTime * damping;
            yield return new WaitForEndOfFrame();
        }
        soundHelper.Value.StopSound(GameSoundTag.SFX_QUAKE);
        transform.position = GetTargetCameraPosition();
    }
}
