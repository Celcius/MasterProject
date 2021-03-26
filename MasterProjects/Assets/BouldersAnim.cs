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


    public void BouldersFalling()
    {
        GameCameraMover cam = ((GameCameraMover)CameraMover.Instance);
        cam.ShakeCamera(smallShakeIntensity, smallShakeDuration, smallShakeDamping, true);

    }

    public void GoToDark()
    {
        darkness.GoToHalfDarkRadius();
    }
}
