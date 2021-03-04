using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsAwarder : RoomChangeHandler
{
    [System.Serializable]
    public struct SkillAward
    {
        public Vector2Int roomAwarded;
        public BoolVar skillUse;
    }

    [SerializeField]
    private SkillAward[] skills;
    
    [SerializeField]
    private RoomCollection rooms;

    public override void OnRoomEnter(Vector2Int pos)
    {
        int index = rooms.GetIndexOfRoom(pos);
        foreach(SkillAward award in skills)
        {
            int awardIndex = rooms.GetIndexOfRoom(award.roomAwarded);
            award.skillUse.Value = (awardIndex < index);
        }

    }

    public override void OnRoomLeave(Vector2Int pos)
    {

    }
}
