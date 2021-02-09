using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cuteable : GridEntity
{
    public void Cut()
    {
        gameObject.SetActive(false);
    }
}
