using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanProjectileController : ProjectileController
{
    private LineRenderer line;
    private int layerMask;
    private const float maxLength = 10000F;
    void Start()
    {
        line = GetComponent<LineRenderer>();
        CalculateLayerMask();
        Vector3 direction = transform.forward;
        RaycastHit hit;
        Vector3[] points = new Vector3[2];
        points[0] = transform.position;
        if(Physics.Raycast(transform.position, direction, out hit, float.PositiveInfinity, layerMask))
        {
            points[1] = hit.point;
        }
        else
        {
            points[1] = transform.position + direction * maxLength;
        }
        line.SetPositions(points);
    }

    private void CalculateLayerMask()
    {
        layerMask = 0;
        foreach (int layer in ignoredLayers)
        {
            layerMask |= (1 << layer);
        }
        layerMask = ~layerMask;
    }
}
