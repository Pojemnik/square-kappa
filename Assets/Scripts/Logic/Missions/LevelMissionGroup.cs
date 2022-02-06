using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelMissionGroup", menuName = "ScriptableObjects/LevelMissionGroup")]
public class LevelMissionGroup : ScriptableObject
{
    [System.Serializable]
    public class MissionListWrapper
    {
        public List<Mission> list;
    }

    public MissionListWrapper mainMisions;
    public List<MissionListWrapper> otherMissions;
}
