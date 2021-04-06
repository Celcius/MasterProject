using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class RoomAnalytics : Singleton<RoomAnalytics>
{
    public struct RoomInfo
    {
        public int retryCount;
        public float startTime;
        public float endTime;
        public float accumulatedTime;
    }

    Dictionary<int, RoomInfo> roomTimers  = new Dictionary<int, RoomInfo>();

    public string startTime;

    private string fileName = "Session_";

    [SerializeField]
    private RoomCollection roomOrder;

    [SerializeField]
    private RoomHandler roomHandler;

    protected void Start()
    {
        System.DateTime now = System.DateTime.Now;
        startTime = $"{now.Day}-{now.Month}-{now.Year}_{now.Hour}-{now.Minute}";
        
        roomHandler.onEnter += OnRoomEnter;
        roomHandler.onLeave += OnRoomLeave;
    }

    protected void OnDestroy() 
    {
        roomHandler.onEnter -= OnRoomEnter;
        roomHandler.onLeave -= OnRoomLeave;
    }
    
    public void OnRoomEnter(Vector2Int pos)
    {
        int index = roomOrder.ContainsRoom(pos)? roomOrder.GetIndexOfRoom(pos) : -1;
        if(index < 0)
        {
            return;
        }
        
        RoomInfo timerStruct;
        if(roomTimers.ContainsKey(index))
        {
            timerStruct = roomTimers[index];
        }
        else
        {
            timerStruct.endTime = 0;
            timerStruct.retryCount = 0;
            timerStruct.accumulatedTime = 0;
        }   
        timerStruct.startTime = Time.time;
        roomTimers[index] = timerStruct;

        PrintRooms();
    }

    public void OnRoomLeave(Vector2Int pos)
    {
        int index = roomOrder.ContainsRoom(pos)? roomOrder.GetIndexOfRoom(pos) : -1;
        FinishedRoom(index);
    }

    public void FinishedRoom(int index)
    {
        if(index < 0)
        {
            return;
        }

        RoomInfo timerStruct;
        if(roomTimers.ContainsKey(index))
        {
            timerStruct = roomTimers[index];
        }
        else
        {
            timerStruct.startTime = Time.time;
            timerStruct.retryCount = 0;
            timerStruct.accumulatedTime = 0;            
        }   
        timerStruct.endTime = Time.time;
        float elapsed = Mathf.Max(0, (timerStruct.endTime - timerStruct.startTime));
        timerStruct.accumulatedTime += elapsed; 
        roomTimers[index] = timerStruct;

        PrintRooms(); 
    }

    public void RetryRoom(Vector2Int roomPos)
    {
        int index = roomOrder.ContainsRoom(roomPos)? roomOrder.GetIndexOfRoom(roomPos) : -1;
        if(index < 0)
        {
            return;
        }
        
        RoomInfo timerStruct;
        if(roomTimers.ContainsKey(index))
        {
            timerStruct = roomTimers[index];
        }
        else
        {
            timerStruct.startTime = Time.time;
            timerStruct.endTime = Time.time;
            timerStruct.retryCount = 0;
            timerStruct.accumulatedTime = 0;
        }    
        timerStruct.retryCount += 1;
        roomTimers[index] = timerStruct;

        PrintRooms(); 
    }

    
    public void PrintRooms()
    {
        string path = Application.dataPath;
        if(!path.EndsWith("/"))
        {
            path += "/";
        }

        path += fileName +"_" + startTime + ".txt"; ;

        string toWrite = "Playtest session ->" + startTime + "\n";
        toWrite += ">>>>>>>>>>>>>>>>>\n";

        int maxIndex = roomOrder.Length;
        
        float totalDuration = 0;
        float totalRetries = 0;
        for(int i = 0; i < maxIndex; i++)
        {
            float duration = 0;
            float retries = 0;

            if(roomTimers.ContainsKey(i))
            {
                duration = roomTimers[i].accumulatedTime;
                retries = roomTimers[i].retryCount;
            }

            totalDuration += duration;
            totalRetries += retries;

            toWrite += $"{duration}\n" ;
        }
        
        toWrite += "---------\n";

        for(int i = 0; i < maxIndex; i++)
        {
            float duration = 0;
            float retries = 0;

            if(roomTimers.ContainsKey(i))
            {
                duration = roomTimers[i].accumulatedTime;
                retries = roomTimers[i].retryCount;
            }

            totalDuration += duration;
            totalRetries += retries;

            toWrite += $"{retries}\n" ;
        }

        toWrite += $"Total:\n{totalDuration}\n";
        toWrite += $"Total:{totalRetries}\n";


     /*   if (!File.Exists(path))
        {
            File.Create(path);
        }*/

        Debug.Log("Writting to " + path);
        StreamWriter writer = new StreamWriter(path);
        writer.Write(toWrite);
        writer.Close();
        Debug.Log (toWrite);
    }
}

