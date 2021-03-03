using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class Cuteable : GridEntity
{
    [SerializeField]
    private Transform cutInstance;

    [SerializeField]
    private RoomTileController controller;

    [SerializeField]
    private GameSoundTag soundTag;

    [SerializeField]
    private SoundHelperVar soundHelperVar;

    public void Cut()
    {
        if(cutInstance != null)
        {
           Instantiate(cutInstance, transform.position, Quaternion.identity); 
        }
        if(soundTag != GameSoundTag.NONE)
        {
            soundHelperVar.Value.PlaySound(soundTag);
        }
        
        gameObject.SetActive(false);
    }
}
