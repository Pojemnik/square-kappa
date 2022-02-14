using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectivesGroup
{
    public enum ObjectivesGroupCompletionMode
    {
        All,
        One,
        Some
    }

    public ObjectivesGroupCompletionMode completionMode;
    public List<string> objectiveNames;
    public MissionEvent objectivesGroupCompleteEvent;
    public UnityEngine.Localization.LocalizedString label;
    public int objectivesNeededToComplete;
}
