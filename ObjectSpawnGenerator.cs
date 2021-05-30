using UnityEngine;
using UnityEditor;
using System.Collections;

public class OrbitSpawnGenerator : EditorWindow
{
    public Vector3 center;
    public int amount;
    public float distance;
    public float positionVariation;
    public float orbitalSpeed;
    public ObjectSpawnData data;
    public Vector3 defaultScale;
    public float scaleVariation;
    public float defaultGravity;
    public float gravityVariation;
    public float angularSpeedVariation;
    public GameObject prefab;
    
    [MenuItem("Window/Orbit Spawn Generator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(OrbitSpawnGenerator));
    }
    void OnGUI()
    {
        GUILayout.Label("Settings", EditorStyles.boldLabel);
        amount = EditorGUILayout.IntField("Amount of objects", amount);
        distance = EditorGUILayout.FloatField("Distance to center", distance);
        positionVariation = EditorGUILayout.FloatField("Distance to center variation", positionVariation);
        orbitalSpeed = EditorGUILayout.FloatField("Orbital speed", orbitalSpeed);
        center = EditorGUILayout.Vector3Field("Gravity center", center);
        defaultScale = EditorGUILayout.Vector3Field("Default scale", defaultScale);
        scaleVariation = EditorGUILayout.FloatField("Scale variation", scaleVariation);
        defaultGravity = EditorGUILayout.FloatField("Default gravity value", defaultGravity);
        gravityVariation = EditorGUILayout.FloatField("Gravity variation", gravityVariation);
        angularSpeedVariation = EditorGUILayout.FloatField("Angular speed variation", angularSpeedVariation);
        data = (ObjectSpawnData)EditorGUILayout.ObjectField(data, typeof(ObjectSpawnData), false);
        prefab = (GameObject)EditorGUILayout.ObjectField(prefab, typeof(GameObject), false);
        if (GUILayout.Button("Generate"))
        {
            if(data == null)
            {
                Debug.LogError("No data object");
                return;
            }
            data.prefab = prefab;
            data.spawnData.Clear();
            for(int i = 0; i < amount; i++)
            {
                ObjectSpawnData.SpawnData temp = new ObjectSpawnData.SpawnData();
                temp.orbitEnabled = true;
                temp.orbitCenter = center;
                temp.initalPosition = center + Random.onUnitSphere * distance + Random.insideUnitSphere * positionVariation;
                float orbitY = Random.Range(0, orbitalSpeed);
                temp.initalSpeed = new Vector3(0, orbitY, (float)System.Math.Sqrt(orbitalSpeed * orbitalSpeed - orbitY * orbitY));
                temp.scale = defaultScale + Random.insideUnitSphere * scaleVariation;
                temp.orbitForceMultipler = defaultGravity + Random.Range(-1, -1) * gravityVariation;
                temp.initalAngularSpeed = Random.insideUnitSphere * angularSpeedVariation;
                data.spawnData.Add(temp);
            }
            Debug.Log("Object spawn data generated correctly");
        }
    }
}