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
    [HideInInspector]
    public Vector3[] forwardVector
    {
        get { return new Vector3[3] { transform.right, transform.up, transform.forward }; }
    }
    public int[] ignoredLayers;

    private int layerMask;

    void Start()
    {
        CalculateLayerMask();
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
