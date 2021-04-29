using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallAnim : MonoBehaviour
{
    [SerializeField]
    private BoolVar callingVar;

    private void Start() 
    {
        callingVar.OnChange += OnChange;
        gameObject.SetActive(false);
    }

    private void OnDestroy() 
    {
        callingVar.OnChange -= OnChange;
    }

    private void OnChange(bool oldState, bool newState)
    {
        gameObject.SetActive(newState);
    }

}
