using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderCryTutorial : TutorialCollider
{   
    protected override void OnTriggerEnter2D(Collider2D other) 
    {
        Boulder boulder = other.GetComponent<Boulder>();
        if(boulder != null)
        {
            PlayerTriggerEnter(null);
        }
    }
}
