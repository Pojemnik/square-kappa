using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AsteroidGenerator))]
public class AsteroidGeneratorEditor : Editor
{
    private SerializedProperty meshSize;
    private SerializedProperty noise;

    private AsteroidGenerator sphere;

    void OnEnable()
    {
        noise = serializedObject.FindProperty("noiseLayers");
        meshSize = serializedObject.FindProperty("meshSize");
        sphere = (AsteroidGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.IntSlider(meshSize, 3, 5);
        EditorGUILayout.PropertyField(noise);
        serializedObject.ApplyModifiedProperties();
        if (GUILayout.Button("Generate"))
        {
            sphere.Generate();
        }
    }
}
