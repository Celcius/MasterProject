using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct TextBalloonString
{
    public string textString;
    public GameConstants.BalloonSpeedEnum speedEnum;

    public float duration;
}
