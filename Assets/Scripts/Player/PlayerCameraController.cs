using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [HideInInspector]
    public GameObject targetItem
    {
        get
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, targettingRange, layerMask))
            {
                return hit.collider.gameObject;
            }
            else
            {
                return null;
            }
        }
    }
    [HideInInspector]
    public float targettingRange;
    public int[] ignoredLayers;

    private int layerMask;

    void Start()
    {
        CalculateLayerMask();
        //layerMask = (1 << 6) | (1 << 7); //do not include player and enemy layers
        //layerMask = ~layerMask;
    }

    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.magenta);
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
