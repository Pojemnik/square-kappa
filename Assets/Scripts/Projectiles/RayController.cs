using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayController : MonoBehaviour
{
    private LineRenderer line;
    private const float maxLength = 10000F;

    public void DisplayRay(Vector3 start, Vector3 direction)
    {
        line.SetPositions(new Vector3[] { start, start + direction * maxLength });
    }

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }
}
