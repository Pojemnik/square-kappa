using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    [Header("Relative")]
    public Vector3 relativePosition;
    public Vector3 relativeRotation;

    [Header("Misc")]
    public Outline outline;
}
