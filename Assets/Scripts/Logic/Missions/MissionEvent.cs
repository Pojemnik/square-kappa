using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionEvent", menuName = "ScriptableObjects/MissionEvent")]
public class MissionEvent : ScriptableObject
{
    [SerializeField]
    private bool displayDebugInfo;

    private List<MissionEventListener> listeners = new List<MissionEventListener>();
    [HideInInspector]
    public event System.EventHandler missionEvent;

    public void Raise()
    {
        if (displayDebugInfo)
        {
            Debug.LogFormat("Mission event {0} called", name);
        }
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised();
        }
        if(missionEvent != null)
        {
            missionEvent(this, null);
        }
    }

    public void RegisterListener(MissionEventListener listener)
    {
        if (displayDebugInfo)
        {
            Debug.LogFormat("Listener {0} for mission event {1} registerd", listener.name, name);
        }
        listeners.Add(listener);
    }

    public void UnregisterListener(MissionEventListener listener)
    {
        if (displayDebugInfo)
        {
            Debug.LogFormat("Listener {0} for mission event {1} unregistered", listener.name, name);
        }
        listeners.Remove(listener);
    }
}
