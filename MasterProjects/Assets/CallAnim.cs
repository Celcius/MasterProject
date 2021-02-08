using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallAnim : MonoBehaviour
{
    [SerializeField]
    private CharacterStateVar characterStateVar;

    private void Start() 
    {
        characterStateVar.OnChange += OnChange;
        gameObject.SetActive(false);
    }

    private void OnDestroy() 
    {
        characterStateVar.OnChange -= OnChange;
    }

    private void OnChange(CharacterState oldState, CharacterState newState)
    {
        if(oldState == newState || (oldState != CharacterState.Calling && newState != CharacterState.Calling))
        {
            return;
        }
        if(oldState == CharacterState.Calling)
        {
            gameObject.SetActive(false);
        }

        if(newState == CharacterState.Calling)
        {
            gameObject.SetActive(true);
        }
    }

}
