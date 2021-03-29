using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BouldersAnim : MonoBehaviour
{
    [SerializeField]
    private DarknessController darkness;

    [SerializeField]
    private float smallShakeIntensity;
    
    [SerializeField]
    private AnimationCurve smallShakeDamping;

    [SerializeField]
    private float smallShakeDuration;

    [SerializeField]
    private MusicPlayer musicPlayer;

    [SerializeField]
    private float musicWaitTime = 3.0f;

    public void BouldersFalling()
    {
        GameCameraMover cam = ((GameCameraMover)CameraMover.Instance);
        cam.ShakeCamera(smallShakeIntensity, smallShakeDuration, smallShakeDamping, true);
        StartCoroutine(PlayMusicAfter(smallShakeDuration));
        musicPlayer.PlayStyle(MusicPlayer.MusicStyle.Silence);
    }

    private IEnumerator PlayMusicAfter(float waitTime)
    {
        yield return new WaitForSeconds(waitTime*musicWaitTime);
        musicPlayer.PlayStyle(MusicPlayer.MusicStyle.Crisis);
    }

    public void GoToDark()
    {
        darkness.GoToHalfDarkRadius();
    }
}
