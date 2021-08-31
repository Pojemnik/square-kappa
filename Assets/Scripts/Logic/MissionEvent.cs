using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionEvent", menuName = "ScriptableObjects/MissionEvent")]
public class MissionEvent : ScriptableObject
{
    private List<MissionEventListener> listeners = new List<MissionEventListener>();

    public void Raise()
    {
        Debug.LogFormat("Mission event {0} called", name);
        for(int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised();
        }
    }

    public void RegisterListener(MissionEventListener listener)
    { 
        listeners.Add(listener); 
    }

    public void UnregisterListener(MissionEventListener listener)
    { 
        listeners.Remove(listener);
    }
}
