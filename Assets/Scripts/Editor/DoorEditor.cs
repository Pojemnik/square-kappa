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

    private void OnEnable()
    {
        lowerPart = serializedObject.FindProperty("lowerPart");
        upperPart = serializedObject.FindProperty("upperPart");
        lowerPartConfig = serializedObject.FindProperty("lowerPartConfig");
        upperPartConfig = serializedObject.FindProperty("upperPartConfig");
        openingTime = serializedObject.FindProperty("openingTime");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(lowerPart);
        EditorGUILayout.PropertyField(upperPart);
        EditorGUILayout.PropertyField(lowerPartConfig);
        EditorGUILayout.PropertyField(upperPartConfig);
        EditorGUILayout.PropertyField(openingTime);

        if(GUILayout.Button("Open"))
        {
            (serializedObject.targetObject as DoorController).Open();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
