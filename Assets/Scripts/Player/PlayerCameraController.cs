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
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, targettingRange, KappaLayerMask.PlayerItemTargetingMask))
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

    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.magenta);
    }
}
