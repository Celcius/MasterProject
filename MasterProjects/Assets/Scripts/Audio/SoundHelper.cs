using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using AmoaebaUtils;
using Sirenix.OdinInspector;

public class SoundHelper : SerializedMonoBehaviour
{   


    public enum MixerGroupTag
    {
        MUSIC,
        MAIN_SFX,
        BG_SFX
    }

    [SerializeField]
    public struct GameSoundDefinition
    {
        public AudioClip audioClip;
        public string audioId;
        public MixerGroupTag groupTag;

    }

    [SerializeField]
    private AudioMixerSoundSystem soundSystem;

    protected AudioMixerGroup MusicGroup => soundSystem.GetGroup(GameConstants.AUDIO_GROUP_MUSIC);
    protected AudioMixerGroup MainEffectsGroup => soundSystem.GetGroup(GameConstants.AUDIO_GROUP_MAIN_SFX);
    protected AudioMixerGroup BGEffectsGroup => soundSystem.GetGroup(GameConstants.AUDIO_GROUP_BG_SFX);

    
    public Dictionary<GameSoundTag, GameSoundDefinition> gameSoundsDict;


    public string PlaySound(GameSoundTag gameSoundId)
    {
        if(!gameSoundsDict.ContainsKey(gameSoundId))
        {
            return null;
        }
        GameSoundDefinition definition =  gameSoundsDict[gameSoundId];

        soundSystem.PlaySound(definition.audioClip, definition.audioId, true, GroupForTag(definition.groupTag));
        return definition.audioId;
    }

    public AudioMixerGroup GroupForTag(MixerGroupTag tag)
    {
        switch(tag)
        {
            case MixerGroupTag.MUSIC:
                return MusicGroup;

            case MixerGroupTag.MAIN_SFX:
                return MainEffectsGroup;

            case MixerGroupTag.BG_SFX:
                return BGEffectsGroup;
        }
        return MainEffectsGroup;
    }

    public void StopSound(GameSoundTag gameSoundTag)
    {
        if(!gameSoundsDict.ContainsKey(gameSoundTag))
        {
            return;
        }
        GameSoundDefinition definition =  gameSoundsDict[gameSoundTag];

        soundSystem.StopSound(definition.audioId);
    }
}
