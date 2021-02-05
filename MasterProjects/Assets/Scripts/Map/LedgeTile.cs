using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.Tilemaps;
 
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LedgeTile : Tile
{

 
#if UNITY_EDITOR
    [MenuItem("Tiles/Ledge Tile")]
    public static void CreateAnimatedTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Ledge Tile", "New Ledge Tile", "asset", "Save Ledge Tile", "Assets");
        if (path == "")
            return;
 
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<LedgeTile>(), path);
    }
#endif
}
