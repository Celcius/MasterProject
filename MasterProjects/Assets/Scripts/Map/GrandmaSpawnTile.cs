using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
 
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GrandmaSpawnTile : Tile
{

#if UNITY_EDITOR
    [MenuItem("Tiles/Grandma Spawn Tile")]
    public static void CreateSpawnTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Spawn Tile", "New Spawn Tile", "asset", "Save Spawn Tile", "Assets");
        if (path == "")
            return;
 
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GrandmaSpawnTile>(), path);
    }
#endif
}
