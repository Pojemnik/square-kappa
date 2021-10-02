using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AsteroidGenerator))]
public class AsteroidGeneratorEditor : Editor
{
    private SerializedProperty noiseMultipler;
    private SerializedProperty baseHeight;
    private SerializedProperty offset;
    private SerializedProperty positionScale;

    private AsteroidGenerator sphere;

    void OnEnable()
    {
        noiseMultipler = serializedObject.FindProperty("noiseMultipler");
        baseHeight = serializedObject.FindProperty("baseHeight");
        offset = serializedObject.FindProperty("offset");
        positionScale = serializedObject.FindProperty("positionScale");
        sphere = (AsteroidGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.Slider(noiseMultipler, 0, 10);
        EditorGUILayout.Slider(baseHeight, 0, 1);
        EditorGUILayout.PropertyField(offset);
        EditorGUILayout.Slider(positionScale, 0, 5);
        sphere.noiseType = (AsteroidGenerator.NoiseType)EditorGUILayout.EnumPopup("Noise type", sphere.noiseType);
        serializedObject.ApplyModifiedProperties();
        if (GUILayout.Button("Generate"))
        {
            sphere.Generate();
        }
    }
}
