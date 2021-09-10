using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectivesGroup
{
    public UnityEngine.Localization.LocalizedString label;
    public List<string> objectiveNames;
    public MissionEvent objectivesGroupCompleteEvent;
}
