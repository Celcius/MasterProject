using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    public const string PLAYER_TAG = "Character";
    public const string GRANDMOTHER_TAG = "Grandmother";
    public const string PUSHABLE_TAG = "Pushable";
    public const string TILEMAP_TAG = "Tilemap";

    public enum BalloonSpeedEnum
    {
        Normal = 0,
        VeryFast,
        Fast,
        Slow,
        VerySlow
    }
    public const float BALLOON_SPEED_VERY_FAST = 0.01f;
    public const float BALLOON_SPEED_FAST = 0.025f;
    public const float BALLOON_SPEED_NORMAL = 0.08f;
    public const float BALLOON_SPEED_SLOW = 0.15f;
    public const float BALLOON_SPEED_VERY_SLOW = 0.2f;

    public static float BallonSpeedFromEnum(BalloonSpeedEnum speedEnum)
    {
        switch (speedEnum)
        {
            case BalloonSpeedEnum.VeryFast:
                return BALLOON_SPEED_VERY_FAST;
            case BalloonSpeedEnum.Fast:
                return BALLOON_SPEED_FAST;
            case BalloonSpeedEnum.Normal:
                return BALLOON_SPEED_NORMAL;
            case BalloonSpeedEnum.Slow:
                return BALLOON_SPEED_SLOW;
            case BalloonSpeedEnum.VerySlow:
                return BALLOON_SPEED_VERY_SLOW;
            default:
                return BALLOON_SPEED_NORMAL;
        }
    }
}
