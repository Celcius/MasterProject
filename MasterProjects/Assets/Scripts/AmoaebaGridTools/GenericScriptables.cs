using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class  GenericScriptables : ScriptableObject
{

    [Header("GhostStuff")]
    public float GhostLayerTransitionTime = 1.0f;

    public float aliveWorldLayerAlpha = 1.0f;
    public float aliveGhostLayerAlpha = 0.5f;
    public float deadGhostLayerAlpha = 1.0f;
    public float deadWorldLayerAlpha = 0.0f;

    public Color targetColor = Color.cyan;
    public Color untargetColor = Color.white;

    public Color possessionColor = Color.red;

    public Color enemyWeaponColor = Color.red;

    public float timeOut = 0.5f;
    public float chargeBlipTime = 0.5f;
    public Color chargeBlipColor = Color.cyan;

    public float animFrameFreq = 3.0f;

    public float contactDamage = 0;
    public float iFrames = 0.2f;
    public float pushBackDamage = 2.0f;
    public float pushBackContact = 1.0f;

    public float pushTime = 0.5f;

    public float minTimeOut = 2.0f;
    public float timeToPossess = 1.0f;

    public string NoCollisionLayer;
    public string AliveLayer;

    public string ChargeLayer;

    public bool usePossessionTime = false;
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.7f;
    public float shakeDampingSpeed = 1.0f;
    
    public bool kickGhostOnDamage = true;
    public float damageToKick = 0;
    public float damageBlipFrequency = 0.1f;
    public Color DamageBlipColor = Color.red;

    public float holeDeathTime = 0.5f;
    public float HoleAttractSpeed = 2.0f;

    public float restartTimer = 3.0f;
}
