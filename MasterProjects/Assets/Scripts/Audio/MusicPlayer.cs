using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using AmoaebaUtils;
using Sirenix.OdinInspector;

public class MusicPlayer : RoomChangeHandler
{
    public enum MusicStyle
    {
        Silence,
        GoldenTime,
        Chase,
        Coping,
        Saudade,
        End
    }
    private struct RoomMusicInfo
    {
        public MusicStyle musicStyle;
        public bool shouldRestartStyle;
    }

    [SerializeField]
    private RoomMusicInfo[] musicPerRoom;

    [SerializeField]
    private RoomCollection roomOrder;

    [SerializeField]
    private AudioMixerSoundSystem soundSystem;
    private const string MUSIC_ID = "MUSIC";

    private MusicStyle currentMusicStyle = MusicStyle.Silence;

    private Dictionary<MusicStyle, int> musicStyleIndex = new Dictionary<MusicStyle, int>();
    [SerializeField]
    private Dictionary<MusicStyle, AudioClip[]> clips = new Dictionary<MusicStyle, AudioClip[]>();

    [SerializeField]
    private SoundHelperVar soundHelper;

    private bool hasPlayedStyle = false;
    protected override void Start()
    {
        base.Start();
        
        System.Array enums = System.Enum.GetValues(typeof(MusicStyle));
        foreach(MusicStyle style in enums)
        {
            musicStyleIndex[style] = 0;
        }
    }

    public override void OnRoomEnter(Vector2Int pos)
    {
        
        int roomIndex = roomOrder.GetIndexOfRoom(pos);
        if(roomIndex >= 0 && roomIndex < musicPerRoom.Length)
        {
            RoomMusicInfo newRoomInfo = musicPerRoom[roomIndex];
            if(newRoomInfo.musicStyle != currentMusicStyle || newRoomInfo.shouldRestartStyle)
            {
                PlayStyle(newRoomInfo.musicStyle);
            }
        }
        else
        {
            PlayStyle(MusicStyle.Silence);
        }
    }

    public override void OnRoomLeave(Vector2Int pos)
    {
    }

    public void PlayStyle(MusicStyle style)
    {
        soundSystem.StopSound(MUSIC_ID);
        if(style == MusicStyle.Silence)
        {
            currentMusicStyle = MusicStyle.Silence;
            return;
        }

        musicStyleIndex[style] = 0;
        currentMusicStyle = style;
        hasPlayedStyle = false;
        NextPlay(MUSIC_ID);
    }

    private void NextPlay(string MusicId)
    {
        if(hasPlayedStyle && currentMusicStyle == MusicStyle.Saudade)
        {
            return;
        }
        
        if(MusicId == MUSIC_ID && !soundSystem.IsPlaying(MUSIC_ID))
        {
            hasPlayedStyle = true;
            soundSystem.PlaySound(GetNextClipForStyle(currentMusicStyle),
                    MUSIC_ID,
                    false,
                    soundHelper.Value.GroupForTag(SoundHelper.MixerGroupTag.MUSIC),
                    NextPlay);
        }
    }

    public AudioClip GetNextClipForStyle(MusicStyle style)
    {
        int index = musicStyleIndex[style];
        AudioClip[] styleClips = clips[style];
        if(styleClips == null || styleClips.Length == 0)
        {
            return null;
        }
        
        AudioClip clip = styleClips[index];
        index = (index+1) % styleClips.Length;
        musicStyleIndex[style] = index;
        return clip;
    }

}
