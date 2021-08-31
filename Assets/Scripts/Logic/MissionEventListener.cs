using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MissionEventListener : MonoBehaviour
{
    public MissionEvent Event;
    public UnityEvent Response;

    private void Awake()
    {
        Event.RegisterListener(this); 
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this); 
    }

    public void OnEventRaised()
    {
        Response.Invoke(); 
    }
}
