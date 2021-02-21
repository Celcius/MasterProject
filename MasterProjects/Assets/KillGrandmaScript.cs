using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillGrandmaScript : PlayerCollideable
{
    public bool hasKilled = false;

    [SerializeField]
    private GrandmaScriptVar grandma;

    [SerializeField]
    private IGrandmaController deadGrandmaController;
    
    protected override void PlayerTriggerEnter(CharacterMovement character) 
    {
        Destroy(grandma.Value.gameObject);
        Instantiate(deadGrandmaController);
    }
    
}
