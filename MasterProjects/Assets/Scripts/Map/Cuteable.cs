using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cuteable : GridEntity
{
    [SerializeField]
    private Transform cutInstance;
    public void Cut()
    {
        if(cutInstance != null)
        {
           Instantiate(cutInstance, transform.position, Quaternion.identity); 
        }
        gameObject.SetActive(false);
    }
}
