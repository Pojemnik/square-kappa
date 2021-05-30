using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public List<ObjectSpawnData> data;
    void Start()
    {
        foreach (ObjectSpawnData objectSpawn in data)
        {
            foreach (ObjectSpawnData.SpawnData spawn in objectSpawn.spawnData)
            {
                GameObject o = Instantiate(objectSpawn.prefab, spawn.initalPosition, Quaternion.identity);
                o.transform.localScale = spawn.scale;
                Rigidbody rb = o.GetComponent<Rigidbody>();
                rb.AddTorque(spawn.initalAngularSpeed, ForceMode.VelocityChange);
                if (spawn.orbitEnabled)
                {
                    OrbitBehaviour orbit = o.GetComponent<OrbitBehaviour>();
                    orbit.enabled = true;
                    //orbit.position = spawn.orbitCenter;
                    //orbit.gravity = spawn.orbitForceMultipler;
                    //orbit.initialSpeed = spawn.initalSpeed;
                }
                else
                {
                    rb.AddForce(spawn.initalSpeed, ForceMode.VelocityChange);
                }

            }
        }
    }
}
