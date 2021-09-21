using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class EventManager : Singleton<EventManager>
{
    [SerializeField]
    private bool logEvents;

    private bool initialized = false;

    public Dictionary<string, UnityEvent> eventDictionary;

    protected EventManager() { }

    private void Awake()
    {
        if (!initialized)
        {
            Init();
        }
    }

    private void Init()
    {
        RegisterInstance(this);
        eventDictionary = new Dictionary<string, UnityEvent>();
        initialized = true;
    }

    public void AddListener(string name, UnityAction action)
    {
        if(!initialized)
        {
            Init();
        }
        UnityEvent unityEvent;
        if (eventDictionary.TryGetValue(name, out unityEvent))
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
        if (!initialized)
        {
            Init();
        }
        if (eventDictionary.TryGetValue(name, out UnityEvent unityEvent))
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
        if (!initialized)
        {
            Init();
        }
        UnityEvent unityEvent;
        if (eventDictionary.TryGetValue(name, out unityEvent))
        {
            unityEvent.Invoke();
            if (logEvents)
            {
                Debug.LogFormat("Event {0} called", name);
            }
        }
        else
        {
            throw new System.Exception(string.Format("Trying to call non-existing event {0}", name));
        }
    }
}
