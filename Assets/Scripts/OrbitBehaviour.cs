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
        Vector3 y, z;
        if (towardsCenter.normalized == Vector3.right || towardsCenter.normalized == Vector3.left)
        {
            y = Vector3.Cross(towardsCenter.normalized, Vector3.up);
            z = Vector3.Cross(towardsCenter.normalized, y);
        }
        else
        {
            y = Vector3.Cross(towardsCenter.normalized, Vector3.right);
            z = Vector3.Cross(towardsCenter.normalized, y);
        }
        rigidbody.AddForce(initialSpeed.x * towardsCenter.normalized, ForceMode.VelocityChange);
        rigidbody.AddForce(initialSpeed.y * y, ForceMode.VelocityChange);
        rigidbody.AddForce(initialSpeed.z * z, ForceMode.VelocityChange);
        rigidbody.AddForce(towardsCenter.normalized * forceMultipler / towardsCenter.sqrMagnitude, ForceMode.Acceleration);
    }

    void FixedUpdate()
    {
        Vector3 towardsCenter = center - rigidbody.position;
        rigidbody.AddForce(Time.fixedDeltaTime * towardsCenter.normalized * forceMultipler / towardsCenter.sqrMagnitude);
    }
}
