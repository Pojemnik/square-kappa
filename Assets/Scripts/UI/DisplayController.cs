using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DisplayController : MonoBehaviour
{
    [SerializeField]
    protected int startValue;

    abstract public void SetValue(int value);
}
