using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DoorController))]
public class DoorEditor : Editor
{
    private SerializedProperty lowerPart;
    private SerializedProperty upperPart;
    private SerializedProperty lowerPartConfig;
    private SerializedProperty upperPartConfig;
    private SerializedProperty openingTime;
    private SerializedProperty defaultState;

    private void OnEnable()
    {
        lowerPart = serializedObject.FindProperty("lowerPart");
        upperPart = serializedObject.FindProperty("upperPart");
        lowerPartConfig = serializedObject.FindProperty("lowerPartConfig");
        upperPartConfig = serializedObject.FindProperty("upperPartConfig");
        openingTime = serializedObject.FindProperty("openingTime");
        defaultState = serializedObject.FindProperty("defaultState");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(lowerPart);
        EditorGUILayout.PropertyField(upperPart);
        EditorGUILayout.PropertyField(lowerPartConfig);
        EditorGUILayout.PropertyField(upperPartConfig);
        EditorGUILayout.PropertyField(openingTime);
        EditorGUILayout.PropertyField(defaultState);

        if(GUILayout.Button("Open"))
        {
            (serializedObject.targetObject as DoorController)?.Open();
        }
        if (GUILayout.Button("Close"))
        {
            (serializedObject.targetObject as DoorController)?.Close();
        }


        serializedObject.ApplyModifiedProperties();
    }
}
