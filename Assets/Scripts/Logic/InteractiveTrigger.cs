using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveTrigger : Trigger, IInteractive
{
    private void Awake()
    {
        if (!gameObject.CompareTag("Interactive"))
        {
            Debug.LogWarningFormat("Object {0} contains interactive component, but isn't tagged properly", gameObject.name);
        }
    }

    public void Interact()
    {
        triggerEvent();
    }
}
