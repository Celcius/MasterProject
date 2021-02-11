using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InvisibleTileSpawn : MonoBehaviour
{
    [SerializeField]
    private TilemapVar tilemapVar;
    protected Tilemap tilemap => tilemapVar.Value;

    private void Start()
    {
        Vector3Int pos = tilemap.WorldToCell(transform.position);
        Color c = tilemap.GetColor(pos);
        c.a = 0;
        tilemap.SetColor(pos, c);
        Destroy(this);
    }
}
