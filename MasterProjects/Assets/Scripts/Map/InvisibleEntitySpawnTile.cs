using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using AmoaebaUtils;
 
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InvisibleEntitySpawnTile : Tile
{
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        Color c = tileData.color;
        c.a = 0;
        tileData.color = c;
        tileData.sprite = null;

        base.GetTileData(position, tilemap, ref tileData);
    }


#if UNITY_EDITOR
    [MenuItem("Tiles/Player Spawn Tile")]
    public static void CreateSpawnTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Spawn Tile", "New Spawn Tile", "asset", "Save Spawn Tile", "Assets");
        if (path == "")
            return;
 
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<InvisibleEntitySpawnTile>(), path);
    }
#endif
}
