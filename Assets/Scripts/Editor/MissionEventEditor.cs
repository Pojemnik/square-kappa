using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MissionEvent))]
public class MissionEventEditor : Editor
{
    private SerializedProperty debugInfo;

    private void OnEnable()
    {
        debugInfo = serializedObject.FindProperty("displayDebugInfo");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(debugInfo);
        if (GUILayout.Button("Send event"))
        {
            (target as MissionEvent).Raise();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
