using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class TearsSoundPlayer : MonoBehaviour
{
    private ParticleSystem  parentParticleSystem;

    private int currentNumberOfParticles = 0;

    [SerializeField]
    public GameSoundTag createSound;

    [SerializeField]
    public GameSoundTag deathSound;

    [SerializeField]
    private SoundHelperVar soundHelper;


    void Start()
    {
        parentParticleSystem = this.GetComponent<ParticleSystem>();
    }


    void Update()
    {
        if (parentParticleSystem.isStopped || parentParticleSystem.isPaused)
        {
            return;
        }

        var amount = Mathf.Abs(currentNumberOfParticles - parentParticleSystem.particleCount);

        if (parentParticleSystem.particleCount < currentNumberOfParticles) 
        { 
            PlaySound(deathSound, amount);
        } 

        if (parentParticleSystem.particleCount > currentNumberOfParticles) 
        { 
            PlaySound(createSound, amount);
        } 

        currentNumberOfParticles = parentParticleSystem.particleCount;
    }

    private void PlaySound(GameSoundTag sound, int amount)
    {
        soundHelper.Value.PlaySound(sound);
    }
}
