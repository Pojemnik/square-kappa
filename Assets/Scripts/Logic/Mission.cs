using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mission", menuName = "ScriptableObjects/Mission")]
public class Mission : ScriptableObject
{
    public string label;
    public List<ObjectivesGroup> groups;
    public MissionEvent missionCompleteEvent;
}
