using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class EventManager : Singleton<EventManager>
{
    private Dictionary<string, UnityEvent> eventDictionary;

    protected EventManager() { }

    private void Awake()
    {
        RegisterInstance(this);
        eventDictionary = new Dictionary<string, UnityEvent>();
    }

    public void AddListener(string name, UnityAction action)
    {
        UnityEvent unityEvent;
        if(eventDictionary.TryGetValue(name, out unityEvent))
        {
            unityEvent.AddListener(action);
        }
        else
        {
            UnityEvent addedEvent = new UnityEvent();
            addedEvent.AddListener(action);
            eventDictionary.Add(name, addedEvent);
        }
    }

    public void RemoveListener(string name, UnityAction action)
    {
        UnityEvent unityEvent;
        if (eventDictionary.TryGetValue(name, out unityEvent))
        {
            unityEvent.RemoveListener(action);
        }
        else
        {
            throw new System.Exception("Trying to remove listener from non-existing event");
        }
    }

    public void TriggerEvent(string name)
    {
        UnityEvent unityEvent;
        if (eventDictionary.TryGetValue(name, out unityEvent))
        {
            unityEvent.Invoke();
        }
        else
        {
            throw new System.Exception("Trying to call non-existing event");
        }
    }
}
