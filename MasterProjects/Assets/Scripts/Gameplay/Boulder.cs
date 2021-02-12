using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class Boulder : Pushable
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Sprite[] sprites;
    private int spriteIndex = 0;
    
    void Start()
    {
        spriteIndex = Random.Range(0,sprites.Length);
        spriteRenderer.sprite = sprites[spriteIndex];

        float[] startRotations = new float[]{0, 90,180,270};
        spriteRenderer.transform.rotation = Quaternion.Euler(0,0,startRotations[Random.Range(0,startRotations.Length)]);    
        //spriteRenderer.transform.localPosition = (Vector3)CameraMover.Instance.CellSize /2.0f;
    }

    protected override void OnPushToPos(Vector3 pos, Vector2Int pushMainDir)
    {
        base.OnPushToPos(pos + CameraMover.Instance.CellSize/2.0f, pushMainDir);
       
        if(!UnityEngineUtils.IsInPlayModeOrAboutToPlay())
        {
            return;
        }

        if(Mathf.Abs(pushMainDir.x) > 0)
        {
            float curRot = spriteRenderer.transform.rotation.eulerAngles.z;
            curRot += pushMainDir.x * -90.0f;
            spriteRenderer.transform.rotation = Quaternion.Euler(0,0, curRot);
        }

        if(Mathf.Abs(pushMainDir.y) > 0)
        {
            spriteIndex = MathUtils.NegMod((spriteIndex + pushMainDir.y), sprites.Length);
            spriteRenderer.sprite = sprites[spriteIndex];
        }
    }

}
