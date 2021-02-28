using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraRotation : MonoBehaviour
{
    [SerializeField]
    private bool invertRotation;

    private Transform cameraTransform;
    private void Start() 
    {
        cameraTransform = CameraMover.Instance.transform;
    }

    private void Update() 
    {
        transform.rotation = invertRotation? Quaternion.Inverse(cameraTransform.rotation) : cameraTransform.rotation;
    }
}
