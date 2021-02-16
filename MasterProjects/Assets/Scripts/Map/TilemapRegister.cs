using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using AmoaebaUtils;

[RequireComponent(typeof(TilemapDivisionTool))]
public class TilemapRegister : MonoBehaviour
{
    [SerializeField]
    private TilemapVar tilemapVar;
    void Start()
    {
        TilemapDivisionTool tool = GetComponent<TilemapDivisionTool>();
        Tilemap map = tool.CollapseMaps(true);
        tilemapVar.Value = map;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
