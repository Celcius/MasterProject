using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using AmoaebaUtils;

public class SpawnRuleTile : RuleTile<Tile>
{

        [SerializeField]
        public GameObject toInstantiate;

        [SerializeField]
        private TilemapVar tilemapVar;

        private GameObject instantiated = null;

        public override void RefreshTile(Vector3Int position, ITilemap tilemap)
        {
            base.RefreshTile(position, tilemap);
        }

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {   
           bool startup = base.StartUp(position, tilemap, go);

           ReInstantiate(position, tilemap);
        
           return startup;
        }

        private void ReInstantiate(Vector3Int position, ITilemap tilemap)
        {
            if(!Application.isPlaying)
            {
                return;
            }
            if(instantiated != null)
            {
                Destroy(instantiated);
            }

            Vector3 cellSize = new Vector3(tilemap.localBounds.size.x / tilemap.size.x,
                                           tilemap.localBounds.size.y / tilemap.size.y,
                                           tilemap.localBounds.size.z / tilemap.size.z);   
            

            Vector3 pos  = Vector3.Scale(position, cellSize)  + cellSize/2.0f;
            
            Instantiate(toInstantiate, 
                        pos,
                        Quaternion.identity, 
                        tilemapVar.Value.gameObject.transform);
        }

}
