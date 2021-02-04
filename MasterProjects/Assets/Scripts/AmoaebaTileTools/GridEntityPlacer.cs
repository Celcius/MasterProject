using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;
using Sirenix.OdinInspector;

[ExecuteInEditMode]
[RequireComponent(typeof(GridEntity))]
public class GridEntityPlacer : SerializedMonoBehaviour

{
    [Delayed]
    [SerializeField]
    [OnValueChanged("UpdatePositionToCellPosition")]
    private Vector3Int cellPosition;

    private Vector3 cachedPosition;
    

    [SerializeField]
    private Grid gridToAdjust;

    private GridEntity entity;

    private void OnInspectorUpdate() 
    {
        if(gridToAdjust == null)
        {
            gridToAdjust = FindObjectOfType<Grid>();
        }
        cellPosition = GetComponent<GridEntity>().GridPos;   
    }

    private void UpdatePositionToCellPosition()
    {
        Vector2 gridSize = Vector2.one;
        if(gridToAdjust != null)
        {
            gridSize = gridToAdjust.cellSize;
        }
    
        GridEntity entity = GetComponent<GridEntity>();
        entity.transform.position = new Vector3(cellPosition.x * gridSize.x,
                                                cellPosition.y * gridSize.y,
                                                cellPosition.z);
        cachedPosition = transform.position;
    }

    private void UpdateCellPositionToPosition()
    {
        Vector2 gridSize = Vector2.one;
        if(gridToAdjust != null)
        {
            gridSize = gridToAdjust.cellSize;
        }

        if(entity == null)
        {
            entity = GetComponent<GridEntity>();
        }
        
        cellPosition = new Vector3Int(Mathf.RoundToInt(transform.position.x / gridSize.x),
                                      Mathf.RoundToInt(transform.position.y / gridSize.y),
                                      Mathf.RoundToInt(transform.position.z));
        UpdatePositionToCellPosition();
        cachedPosition = transform.position;
    }

    private void Update()
    {
        if(UnityEngineUtils.IsInPlayModeOrAboutToPlay())
        {
            return;
        }

        if(transform.position != cachedPosition)
        {
            UpdateCellPositionToPosition();
        }
    }
}


