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
    private IGrandmaController defaultGrandmaController;

   [SerializeField]
    private Vector2Var resetPos;

    [SerializeField]
    private bool SpawnOnResetPos = false;

    private bool playerIsOn = false;

    [SerializeField]
    private bool OncePerRoom = false;
    private bool hasSpawnedOnce = false;

    [SerializeField]
    private Vector2Int roomPos;

    [SerializeField]
    private RoomHandler roomHandler;
    
    private void Start() 
    {
        roomHandler.onEnter += OnRoomEnter;
        roomHandler.onLeave += OnRoomLeave;
    }

    private void OnDestroy() 
    {
        roomHandler.onEnter -= OnRoomEnter;
        roomHandler.onLeave -= OnRoomLeave;
    }

    protected override void EntityTriggerExit(CharacterMovement character) 
    {
        playerIsOn = false;
    }

    protected override void EntityTriggerEnter(CharacterMovement character) 
    {
        playerIsOn = true;

        if(grandma.Value.GetType() == specialGrandmaController.GetType())
        {
            return;
        }

        SwapGrandmother();
    }

    private void SwapGrandmother()
    {
        if(OncePerRoom && hasSpawnedOnce)
        {
            return;
        }
        
        Vector2Int intResetPos = new Vector2Int(Mathf.FloorToInt(resetPos.Value.x), 
                                                Mathf.FloorToInt(resetPos.Value.y));
        Vector3 pos = (SpawnOnResetPos || grandma.Value == null?
                        CameraMover.WorldPosForGridPos((Vector3Int)intResetPos, 0) :
                        grandma.Value.transform.position);
        Destroy(grandma.Value.gameObject);
        IGrandmaController spawned = Instantiate<IGrandmaController>(specialGrandmaController);
        spawned.transform.position = pos;
        grandma.Value = spawned;
        hasSpawnedOnce = true;
    }

    public virtual void OnRoomEnter(Vector2Int pos)
    {
        if(pos == roomPos)
        {
            hasSpawnedOnce = false;
            SwapGrandmother();
        }
    }

    public virtual void OnRoomLeave(Vector2Int pos)
    {
        if(pos == roomPos)
        {
            hasSpawnedOnce = false;
            if(grandma.Value.GetType() == specialGrandmaController.GetType())
            {
                Vector3 position = grandma.Value.transform.position;
                Destroy(grandma.Value.gameObject);
                IGrandmaController defaultGran = Instantiate<IGrandmaController>(defaultGrandmaController);
                defaultGran.transform.position = position;
            }
        }
    }
    
}
