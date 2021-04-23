using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitBehaviour : MonoBehaviour
{
    public Vector3 center;
    public Vector3 initialSpeed; //x - towards center (radial), y - global up, z - the other one
    public float forceMultipler;

    private new Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Vector3 towardsCenter = center - rigidbody.position;
        rigidbody.AddForce(initialSpeed.x * towardsCenter.normalized, ForceMode.VelocityChange);
        rigidbody.AddForce(initialSpeed.y * Vector3.up, ForceMode.VelocityChange);
        rigidbody.AddForce(initialSpeed.z * Vector3.Cross(Vector3.up, towardsCenter.normalized), ForceMode.VelocityChange);
        rigidbody.AddForce(towardsCenter.normalized * forceMultipler / towardsCenter.sqrMagnitude, ForceMode.Acceleration);
    }

    void FixedUpdate()
    {
        Vector3 towardsCenter = center - rigidbody.position;
        rigidbody.AddForce(Time.fixedDeltaTime * towardsCenter.normalized * forceMultipler / towardsCenter.sqrMagnitude);
    }
}
