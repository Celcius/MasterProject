using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameCameraMover : CameraMover
{
    CharacterMovement movement;

    [SerializeField]
    private SoundHelperVar soundHelper;

    [SerializeField]
    private float magnitudeSoundThreshold = 0.8f;

    [SerializeField]
    private string quakeVolume = "QuakeVolume";

    [SerializeField]
    private AnimationCurve quakeVolumeRange;

    [SerializeField]
    private AudioMixerSnapshot shakeSnapshot;

    [SerializeField]
    private AudioMixerSnapshot normalSnapshot;

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

    public override void ShakeCamera(float intensity, float duration, float damping = 1.0f)
    {
        AnimationCurve dampingCurve = new AnimationCurve();
        dampingCurve.AddKey(0, damping);
        dampingCurve.AddKey(1.0f, damping);
        ShakeCamera(intensity, duration, dampingCurve, false);
    }

    public void ShakeCamera(float intensity, float duration, AnimationCurve damping, bool playSound = false)
    {
        if(Moving)
        {
            return;
        }

        transform.position = GetTargetCameraPosition();
        StopShake();
        cameraShake = CameraShakeSound(duration,
                                  intensity * shakeMagnitude,
                                  damping,
                                  playSound);
        shakeSnapshot.TransitionTo(3.0f);
        StartCoroutine(cameraShake);
    }


    protected override void StopShake()
    {
        base.StopShake();
        normalSnapshot.TransitionTo(3.0f);
    }

    protected IEnumerator CameraShakeSound(float time, float magnitude, AnimationCurve damping, bool playSound)
    {
        Vector3 initialPosition = GetTargetCameraPosition();
        float duration = time;
        while(duration >= 0)
        {
            if(playSound) 
            {
                PlayQuakeSound(magnitude);
            }
            float ratio = Mathf.Clamp01((time-duration) / time);
            float dampingVal = damping.Evaluate(ratio);
            soundHelper.Value.SetMixerFloat(quakeVolume, quakeVolumeRange.Evaluate(dampingVal));
            transform.localPosition = initialPosition + UnityEngine.Random.insideUnitSphere * magnitude * dampingVal;
        
            duration -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        soundHelper.Value.StopSound(GameSoundTag.SFX_QUAKE);
        transform.position = GetTargetCameraPosition();
        normalSnapshot.TransitionTo(3.0f);
    }

    private void PlayQuakeSound(float magnitude)
    {
        if(magnitude >= magnitudeSoundThreshold)
        {
            if(soundHelper.Value.IsPlaying(GameSoundTag.SFX_LARGE_QUAKE))
            {
                return;
            }
            soundHelper.Value.StopSound(GameSoundTag.SFX_QUAKE);
            if(!soundHelper.Value.IsPlaying(GameSoundTag.SFX_LARGE_QUAKE))
            {
                soundHelper.Value.PlaySound(GameSoundTag.SFX_LARGE_QUAKE,false);
            }    
        } 
        else 
        {
            if(soundHelper.Value.IsPlaying(GameSoundTag.SFX_QUAKE))
            {
                return;
            }
            soundHelper.Value.StopSound(GameSoundTag.SFX_LARGE_QUAKE);
            if(!soundHelper.Value.IsPlaying(GameSoundTag.SFX_QUAKE))
            {
                soundHelper.Value.PlaySound(GameSoundTag.SFX_QUAKE, true);
            }
        } 
    }
}
