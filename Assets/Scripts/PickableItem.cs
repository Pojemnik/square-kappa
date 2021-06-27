using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickableItem : MonoBehaviour
{
    [Header("Relative")]
    public Vector3 relativePosition;
    public Vector3 relativeRotation;

    public IEnumerator SetLayerAfterDelay(float time, int layer)
    {
        yield return new WaitForSeconds(time);
        gameObject.layer = layer;
    }
}
