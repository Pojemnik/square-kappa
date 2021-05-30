using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StartSpeedBehaviour : MonoBehaviour
{
    public Vector3 startSpeed;
    public Vector3 startRotation;

    private new Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();   
    }

    void Start()
    {
        rigidbody.AddRelativeForce(startSpeed, ForceMode.VelocityChange);
        rigidbody.AddRelativeTorque(startRotation, ForceMode.VelocityChange);
    }
}
