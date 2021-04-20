using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ObjectSpawnData", order = 1)]
public class ObjectSpawnData : ScriptableObject
{
    [System.Serializable]
    public class SpawnData
    {
        public Vector3 initalPosition;
        public Vector3 initalSpeed = Vector3.zero;
        public Vector3 initalAngularSpeed;
        public Vector3 scale = Vector3.one;
    }
    public GameObject prefab;
    public List<SpawnData> spawnData;
}
