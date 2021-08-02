using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AsteroidGenerator))]
public class AsteroidGeneratorEditor : Editor
{
    private SerializedProperty bottomHeight;
    private SerializedProperty minCraterSize;
    private SerializedProperty maxCraterSize;
    private SerializedProperty size;
    private SerializedProperty smoothK;
    private SerializedProperty cratersNumber;
    private SerializedProperty seed;
    private SerializedProperty hillRatio;
    private SerializedProperty hillHeight;

    private AsteroidGenerator sphere;
    private bool cratersOptions = true;
    private float craterMin = 0.05f;
    private float craterMax = 0.5f;

    void OnEnable()
    {
        bottomHeight = serializedObject.FindProperty("bottomHeight");
        minCraterSize = serializedObject.FindProperty("minCraterSize");
        maxCraterSize = serializedObject.FindProperty("maxCraterSize");
        size = serializedObject.FindProperty("meshSize");
        smoothK = serializedObject.FindProperty("smoothK");
        cratersNumber = serializedObject.FindProperty("craters");
        seed = serializedObject.FindProperty("seed");
        hillRatio = serializedObject.FindProperty("hillRatio");
        hillHeight = serializedObject.FindProperty("hillHeight");
        sphere = (AsteroidGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(seed);
        EditorGUILayout.IntSlider(size, 3, 6);
        EditorGUILayout.Slider(smoothK, 0, 1, "Smoothness");
        cratersOptions = EditorGUILayout.Foldout(cratersOptions, "Craters");
        if (cratersOptions)
        {
            EditorGUILayout.Slider(bottomHeight, 0, 1);
            EditorGUILayout.Slider(hillRatio, 0, 1);
            EditorGUILayout.Slider(hillHeight, bottomHeight.floatValue, 1);
            EditorGUILayout.IntSlider(cratersNumber, 0, 100);
            EditorGUILayout.LabelField("Min crater size:", craterMin.ToString());
            EditorGUILayout.LabelField("Max crater size:", craterMax.ToString());
            EditorGUILayout.MinMaxSlider("Craters sizes", ref craterMin, ref craterMax, 0, 1);
            minCraterSize.floatValue = craterMin;
            maxCraterSize.floatValue = craterMax;
            sphere.craterOverlap = (AsteroidGenerator.CraterOverlapMode)EditorGUILayout.EnumPopup("Crater overlap mode", sphere.craterOverlap);
        }
        serializedObject.ApplyModifiedProperties();
        if (GUILayout.Button("Generate"))
        {
            sphere.Generate();
        }
    }
}
