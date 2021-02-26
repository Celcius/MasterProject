using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeGrandmother : AnimTutorialGrandmother
{

    [SerializeField]
    private string tutorialLabel = "Press 'E' to cut...";

    [SerializeField]
    private StringVar tutorialStringVar;

    [SerializeField]
    private GridEntityVar player;

    private bool moveToPlayer = false;

    [SerializeField]
    private BoolVar canCut;

    [SerializeField]
    private float moveSpeed = 5;

    public override void OnAnimationEnded() 
    {
        moveToPlayer = true;
        GetComponent<Animator>().enabled = false;
    }

    private void Update() 
    {
        if(!moveToPlayer)
        {
            return;
        }

        Vector2 dir = (player.Value.transform.position - transform.position).normalized;
        transform.position += (Vector3)dir * Time.deltaTime * moveSpeed;
    }


    private void OnTriggerEnter2D(Collider2D other) 
    {       
        if(moveToPlayer && other.tag == GameConstants.PLAYER_TAG)
        {
            canCut.Value = true;
            GrandmaController grandma = CreateGrandmaReplacement();
            ShowNextText(grandma.Balloon);
            tutorialStringVar.Value = tutorialLabel;
        }
    }


}
 