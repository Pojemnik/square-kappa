using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission
{
    public string label;
    public List<ObjectivesGroup> groups;
    public UnityEngine.Events.UnityEvent missionCompleteEvent;
}
