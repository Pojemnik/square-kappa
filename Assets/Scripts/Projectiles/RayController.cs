using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayController : MonoBehaviour
{
    private LineRenderer line;
    private const float maxLength = 10000F;
    private Vector3 localStartPoint;

    public Vector3 StartPoint { get => transform.TransformPoint(localStartPoint); }

    public void SetLocalRayDirection(Vector3 start, Vector3 direction)
    {
        localStartPoint = start;
        line.SetPositions(new Vector3[] { start, start + transform.InverseTransformDirection(direction) * maxLength });
    }

    public void SetLocalRayDirection(Vector3 direction)
    {
        line.SetPosition(1, transform.InverseTransformDirection(direction) * maxLength);
    }

    public void SetRayEnd(Vector3 rayEnd)
    {
        line.SetPosition(1, rayEnd);
    }

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = false;
    }
}
