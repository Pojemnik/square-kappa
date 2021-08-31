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

    private void OnEnable()
    {
        logEvents = serializedObject.FindProperty("logEvents");
    }

    public override void OnInspectorGUI()
    {
        if ((target as EventManager).eventDictionary != null)
        {
            events = (target as EventManager).eventDictionary.Keys.ToList();
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
            (target as EventManager).TriggerEvent(eventName);
        }
        if (events != null)
        {
            groupToggle = EditorGUILayout.Foldout(groupToggle, "Registerd events");
            if (groupToggle)
            {
                foreach (string e in events)
                {
                    EditorGUILayout.LabelField(e);
                }
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
