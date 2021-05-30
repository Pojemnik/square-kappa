using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitBehaviour : MonoBehaviour
{
    [Header("Gravity center")]
    public Vector3 position;
    public bool globalPosition;
    public float gravity;

    [Header("Orbit parameters")]
    [Tooltip("x - towards center (radial), y - global up, z - the other one")]
    public Vector3 initialSpeed;

    private new Rigidbody rigidbody;
    private Vector3 globalPositionOfCenter;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (globalPosition)
        {
            globalPositionOfCenter = position;
        }
        else
        {
            globalPositionOfCenter = position + rigidbody.position;
        }
        Vector3 towardsCenter = globalPositionOfCenter - rigidbody.position;
        Plane normalPlane = new Plane(towardsCenter.normalized, rigidbody.position);
        Vector3 y, z;
        if (towardsCenter != Vector3.up && towardsCenter != Vector3.down)
        {
            y = (normalPlane.ClosestPointOnPlane(rigidbody.position + Vector3.up) - rigidbody.position).normalized;
            z = Vector3.Cross(towardsCenter.normalized, y).normalized;
        }
        else
        {
            System.Exception exception = new System.Exception("Vector towards center is y or -y");
            Debug.LogException(exception);
            throw exception; 
        }
        rigidbody.AddForce(initialSpeed.x * towardsCenter.normalized, ForceMode.VelocityChange);
        rigidbody.AddForce(initialSpeed.y * y, ForceMode.VelocityChange);
        rigidbody.AddForce(initialSpeed.z * z, ForceMode.VelocityChange);
        rigidbody.AddForce(towardsCenter.normalized * gravity / towardsCenter.sqrMagnitude, ForceMode.Acceleration);
    }

    void FixedUpdate()
    {
        Vector3 towardsCenter = globalPositionOfCenter - rigidbody.position;
        rigidbody.AddForce(Time.fixedDeltaTime * towardsCenter.normalized * gravity / towardsCenter.sqrMagnitude);
    }
}
