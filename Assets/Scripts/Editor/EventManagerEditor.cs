using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(EventManager))]
public class EventManagerEditor : Editor
{
    private SerializedProperty logEvents;
    private List<string> events;
    private string eventName;
    private bool groupToggle;
    private EventManager manager;

    private void OnEnable()
    {
        logEvents = serializedObject.FindProperty("logEvents");
        manager = (EventManager)target;
    }

    public override void OnInspectorGUI()
    {
        if(manager == null)
        {
            manager = (EventManager)target;
        }
        if (manager.eventDictionary != null)
        {
            events = manager.eventDictionary.Keys.ToList();
        }
        else
        {
            if (events == null)
            {
                events = new List<string>();
            }
        }
        serializedObject.Update();
        EditorGUILayout.PropertyField(logEvents);
        eventName = EditorGUILayout.TextField(eventName);
        if (GUILayout.Button("Send event"))
        {
            manager.TriggerEvent(eventName);
        }
        if (events != null)
        {
            groupToggle = EditorGUILayout.Foldout(groupToggle, "Registerd events");
            if (groupToggle)
            {
                foreach (string e in events)
                {
                    if(GUILayout.Button(e))
                    {
                        manager.TriggerEvent(e);
                    }
                }
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
