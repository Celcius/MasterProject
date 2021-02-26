using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGrandmother: AnimTutorialGrandmother
{
    [SerializeField]
    private GrandmaController nextGrandma;
    public override void OnAnimationEnded() 
    {
        GrandmaController grandma = Instantiate<GrandmaController>(nextGrandma, transform.position, transform.rotation);
        ShowNextText(grandma.Balloon);
        Destroy(this.gameObject);
    }
}
 