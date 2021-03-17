using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightScript : RoomChangeHandler
{
    [System.Serializable]
    private struct HeightDef
    {
        public string heightString;
        public bool shouldShow;
    }

    [SerializeField]
    private StringVar varToShow;

    [SerializeField]
    private RoomCollection roomOrder;

    [SerializeField]
    private HeightDef[] stringToShow;

    public override void OnRoomEnter(Vector2Int pos)
    {
        ShowStringForRoom(pos);
    }

    public override void OnRoomLeave(Vector2Int pos) { }

    public void ShowStringForRoom(Vector2Int pos)
    {
        if(!roomOrder.ContainsRoom(pos))
        {
            Debug.LogError("Unknown room " + pos);
            return;
        }
        
        int index = roomOrder.GetIndexOfRoom(pos);

        if(stringToShow.Length <= index)
        {
            Debug.LogError("Unknown heightString " + pos);
            return;
        }

        if(stringToShow[index].shouldShow)
        {
            varToShow.Value = stringToShow[index].heightString;
        }
        else
        {
            varToShow.Value = "";
        }

    }
}
