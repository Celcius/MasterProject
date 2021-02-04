using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using AmoaebaUtils;
using Sirenix.OdinInspector;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class VerticalWorldController : MapScriptable
{
   private HashSet<Vector3Int> topPositions = new HashSet<Vector3Int>();
   public HashSet<Vector3Int> TopPositions => topPositions;

   private Vector3Int topPosMin;
   private Vector3Int topPosMax;
   private int flooringIndex;

   private Tilemap FloorMap => flooringIndex >= tileMaps.Value.Length? null : tileMaps.Value[flooringIndex];

   [SerializeField]
   private LayerMaskVar worldMask;
   public LayerMask WorldMask => worldMask.Value;

   [SerializeField]
   private bool IsTilemapWall = false;

   [SerializeField]
   private bool ignoreZAxis = false;

   protected override void OnEnable()
   {
       base.OnEnable();
       ProcessMap();
   }

   public override void OnMapChange(Tilemap[] oldMap, Tilemap[] newMap)
   {
       if(!UnityEngineUtils.IsInPlayModeOrAboutToPlay())
        {
            return;
        }
        
       ProcessMap();
   }

#if UNITY_EDITOR
   [MenuItem("Tilemap/World")]
   private static void SelectWorld()
   {
       UnityEngineUtils.SelectFirstObjectOfType<VerticalWorldController>();
   }
#endif


   private void ProcessMap()
   {
       topPositions.Clear();

       for(int i = 0; i < tileMaps.Value.Length; i++)
       {
           Tilemap map = tileMaps.Value[i];
           if((WorldMask.value & (1 << map.gameObject.layer)) != 0)
           {
               flooringIndex = i;
               CalculateTopTiles(TilemapUtils.GetTileIndexes(map));
               break;
           }
       }
   }

   private void CalculateTopTiles(Vector3Int[] positions)
   {
       ArrayList toRemove = new ArrayList();
       foreach(Vector3Int pos in positions)
       {
           toRemove.Clear();
           bool hasAbove = false;

           if(!ignoreZAxis)
           {
                foreach(Vector3Int topPos in topPositions)
                {
                    if(TilemapUtils.IsPosAbove(pos, topPos))
                    {
                        hasAbove = true;
                        break;
                    }
                    else if(TilemapUtils.IsPosBelow(pos, topPos))
                    {
                        toRemove.Add(topPos);
                    }
                }
                
           }
           
           if(!hasAbove)
           {
               topPositions.Add(pos);
               foreach(Vector3Int posBelow in toRemove)
               {
                    topPositions.Remove(posBelow);
               } 
           }
       }

        topPosMax = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
        topPosMin = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);

        foreach(Vector3Int topPos in topPositions)
        {
            topPosMax = Vector3Int.Max(topPosMax, topPos);
            topPosMin = Vector3Int.Min(topPosMax, topPos);
        }
   }

   public bool GetAboveCell(Vector3 worldPosition, out Vector3Int cellPoint, int searchBreath)
   {
        Tilemap map = FloorMap;
        Vector3Int cellPos = map.WorldToCell(worldPosition);
        
        for(int i = searchBreath; i >= 0; i--)
        {
            Vector3Int checkCell = cellPos + new Vector3Int(0, 0, i);
            if(TopPositions.Contains(checkCell))
            {
                cellPoint = checkCell;
                return true;
            }   
        }
    
       cellPoint = Vector3Int.zero;
       return false;
   }

    public bool IsFloorCell(Vector3 worldPosition)
   {
       return IsFloorCell(worldPosition, out _);
   }

   public bool IsFloorCell(Vector3 worldPosition, out Vector3Int cellPoint)
   {
       bool isFloorCell = GetAboveCell(worldPosition, out cellPoint, 0);
       return IsTilemapWall? !isFloorCell : isFloorCell;
   }

   public Vector3 CellToWorld(Vector3Int pos)
   {
        return FloorMap.CellToWorld(pos);
   }

   public Vector3Int WorldToCell(Vector3 pos)
   {
        return FloorMap.WorldToCell(pos);
   }

   public Vector3 ClampToWorldPos(Vector3 pos)
   {
       return CellToWorld(WorldToCell(pos));
   }

#region StoreMaps

#if UNITY_EDITOR
   private const string MapLocation = "Assets/Scriptables/Maps/";

   [PropertySpace(32)]
   [HorizontalGroup("StoreMap", 0.7f, PaddingRight = 15)]
   [SerializeField]
   private string storeMapLabel;

   [PropertySpace(32)]
   [HorizontalGroup("StoreMap", 0.3f, PaddingRight = 15)]
   [Button]
   private void StoreMap()
   {       
       if(storeMapLabel.Length > 0)
       {
           Debug.Log("Storing Map as " + storeMapLabel);
           string fullPath = MapLocation + storeMapLabel + ".asset";

           ComputedWorld entity = AssetDatabase.LoadAssetAtPath<ComputedWorld>(fullPath);
           if(entity != null)
           {
               if(EditorUtility.DisplayDialog("File Already Exists", 
                                              $"The file '{storeMapLabel}'.asset already exists, do you want to overwrite it?",
                                              "Overwrite", 
                                              "Cancel"))
               {
                   AssetDatabase.DeleteAsset(fullPath);
               }
           }
           
           ProcessMap();
           entity = ScriptableObject.CreateInstance<ComputedWorld>();
           entity.Compute(FloorMap, TopPositions);

           ScriptableObjectUtility.CreateAssetAtPath(entity, fullPath);
       }
   }

    private Color GetColor() { return tileMaps.Count() == 0 ? Color.red : Color.green; }


   [Button, GUIColor("GetColor")]
   private void UpdateWorld()
   {
       RegisterTilemapArrayVar[] registers = FindObjectsOfType<RegisterTilemapArrayVar>();
       foreach(RegisterTilemapArrayVar register in registers)
       {
           register.Unregister();
           register.Register();
       }
       ProcessMap();
   }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void UpdateWorldWhenReady()
    {
        if(EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            EditorApplication.delayCall += UpdateWorldWhenReady;
            return;
        }
    
        VerticalWorldController[] controllers = UnityEngineUtils.GetAllOfType<VerticalWorldController>();
        foreach(VerticalWorldController controller in controllers)
        {
            controller.UpdateWorld();
        }
    }

#endif
#endregion
}