using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectivesGroup
{
    public List<string> objectiveNames;
    public MissionEvent objectivesGroupCompleteEvent;
    public UnityEngine.Localization.LocalizedString label;
}
