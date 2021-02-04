using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;

public abstract class MapScriptable : SerializedScriptableObject
{
   [SerializeField]
   protected TilemapArrayVar tileMaps;


   protected virtual void OnEnable()
   {
       tileMaps.OnChange += OnMapChange;
   }

   protected virtual void OnDisable() 
   {     
       tileMaps.OnChange -= OnMapChange; 
   }

   public abstract void OnMapChange(Tilemap[] oldMap, Tilemap[] newMap);
}
