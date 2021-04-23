using UnityEngine;
using UnityEditor;
using System.Collections;

public class OrbitSpawnGenerator : EditorWindow
{
    public Vector3 center;
    public int amount;
    public float distance;
    public float orbitalSpeed;
    public ObjectSpawnData data;
    
    [MenuItem("Window/Orbit Spawn Generator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(OrbitSpawnGenerator));
    }
    void OnGUI()
    {
        GUILayout.Label("Settings", EditorStyles.boldLabel);
        amount = EditorGUILayout.IntField("Amount of objects", 0);
        distance = EditorGUILayout.FloatField("Distance to center", 0);
        orbitalSpeed = EditorGUILayout.FloatField("Orbital speed", 0);
        center = EditorGUILayout.Vector3Field("Gravity center", Vector3.zero);
        data = (ObjectSpawnData)EditorGUILayout.ObjectField(data, typeof(ObjectSpawnData), false);
        if(GUILayout.Button("Generate"))
        {
            Debug.Log("Pressed");
        }
    }
}