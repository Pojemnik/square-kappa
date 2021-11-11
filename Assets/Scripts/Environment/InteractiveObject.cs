using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    private void Awake()
    {
        if(!gameObject.CompareTag("Interactive"))
        {
            Debug.LogWarningFormat("Object {0} contains interactive component, but isn't tagged properly", gameObject.name);
        }
    }

    public void Interact()
    {
        Debug.LogFormat("Player interacted with {0}", gameObject.name);
    }
}
