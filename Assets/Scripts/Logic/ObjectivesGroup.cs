using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectivesGroup
{
    public string label;
    public List<string> objectiveNames;
    [HideInInspector]
    public List<Objective> objectives;
    public MissionEvent objectivesGroupCompleteEvent;
}
