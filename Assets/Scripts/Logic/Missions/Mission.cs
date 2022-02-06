using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mission", menuName = "ScriptableObjects/Mission")]
public class Mission : ScriptableObject
{
    public UnityEngine.Localization.LocalizedString label;
    public MissionEvent missionCompleteEvent;
    public List<ObjectivesGroup> groups;
}
