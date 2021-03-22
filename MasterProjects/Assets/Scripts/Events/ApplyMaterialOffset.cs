using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ApplyMaterialOffset : MonoBehaviour
{
    private Renderer render;
    private Material[] materials;

    [SerializeField]
    private Vector2 offsetSpeed;
    void Start()
    {
        render = GetComponent<Renderer>();
        materials = render.materials;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach(Material mat in materials)
        {
            Vector2 offset = mat.mainTextureOffset;
            mat.mainTextureOffset =  offset + offsetSpeed * Time.deltaTime;
        }
    }
}
