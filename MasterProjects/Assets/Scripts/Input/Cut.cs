using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cut : MonoBehaviour
{
   private void OnTriggerEnter2D(Collider2D other) 
   {
        Cuteable cuteable = other.GetComponent<Cuteable>();
        if(cuteable != null)
        {
            cuteable.Cut();
        }
    }
}
