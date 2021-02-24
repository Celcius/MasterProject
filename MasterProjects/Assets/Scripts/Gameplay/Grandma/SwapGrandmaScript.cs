using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapGrandmaScript : PlayerCollideable
{
    [SerializeField]
    private GrandmaScriptVar grandma;

    [SerializeField]
    private IGrandmaController specialGrandmaController;

   [SerializeField]
    private Vector2Var resetPos;

    [SerializeField]
    private bool SpawnOnResetPos = false;

    private bool playerIsOn = false;
    
    private void Start() 
    {
        
    }

    private void OnDestroy() 
    {
           
    }

    protected override void PlayerTriggerExit(CharacterMovement character) 
    {
        playerIsOn = false;
    }

    protected override void PlayerTriggerEnter(CharacterMovement character) 
    {
        playerIsOn = true;

        if(grandma.Value.GetType() == specialGrandmaController.GetType())
        {
            return;
        }

        Vector2Int intResetPos = new Vector2Int(Mathf.FloorToInt(resetPos.Value.x), 
                                                Mathf.FloorToInt(resetPos.Value.y));
        Vector3 pos = (SpawnOnResetPos || grandma.Value == null?
                        CameraMover.WorldPosForGridPos((Vector3Int)intResetPos, 0) :
                        grandma.Value.transform.position);
        Destroy(grandma.Value.gameObject);
        Instantiate(specialGrandmaController);
        specialGrandmaController.transform.position = pos;
    }
    
}
