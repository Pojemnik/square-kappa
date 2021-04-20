using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public List<ObjectSpawnData> data;
    void Start()
    {
        foreach(ObjectSpawnData objectSpawn in data)
        {
            foreach(ObjectSpawnData.SpawnData spawn in objectSpawn.spawnData)
            {
                GameObject o = Instantiate(objectSpawn.prefab, spawn.initalPosition, Quaternion.identity);
                o.transform.localScale = spawn.scale;
                Rigidbody rb = o.GetComponent<Rigidbody>();
                rb.AddForce(spawn.initalSpeed, ForceMode.VelocityChange);
                rb.AddTorque(spawn.initalAngularSpeed, ForceMode.VelocityChange);
            }
        }
    }
}
