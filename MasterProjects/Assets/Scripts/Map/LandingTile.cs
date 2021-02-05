using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.Tilemaps;
 
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LandingTile : Tile
{

 
#if UNITY_EDITOR
    [MenuItem("Tiles/Landing Tile")]
    public static void CreateLandingTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Landing Tile", "New Landing Tile", "asset", "Save Landing Tile", "Assets");
        if (path == "")
            return;
 
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<LandingTile>(), path);
    }
#endif
}
