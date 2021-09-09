using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickableItem : MonoBehaviour
{
    [Header("Relative")]
    public Vector3 relativePosition;
    public Vector3 relativeRotation;

    [HideInInspector]
    public event System.EventHandler<PickableItem> PickedUp;
    [HideInInspector]
    public event System.EventHandler<PickableItem> Dropped;

    public void OnPickup()
    {
        if (PickedUp != null)
        {
            PickedUp(this, this);
        }
    }

    public void OnDrop()
    {
        if (Dropped != null)
        {
            Dropped(this, this);
        }
    }

    public IEnumerator SetLayerAfterDelay(float time, int layer)
    {
        yield return new WaitForSeconds(time);
        gameObject.layer = layer;
    }
}
