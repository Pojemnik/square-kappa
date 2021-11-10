using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public float targettingRange;
    [SerializeField]
    private float wallRange;
    [HideInInspector]
    public event System.EventHandler<float> wallCloseEvent;

    private bool lastWallClose;

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

    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.magenta);
        if (wallCloseEvent == null)
        {
            return;
        }
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, wallRange, KappaLayerMask.PlayerItemTargetingMask))
        {
            lastWallClose = true;
            wallCloseEvent?.Invoke(this, hit.distance);
        }
        else
        {
            if (lastWallClose)
            {
                wallCloseEvent?.Invoke(this, -1);
            }
        }
    }
}
