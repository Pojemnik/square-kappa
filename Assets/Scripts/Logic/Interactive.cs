using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : MonoBehaviour, IInteractive
{
    [SerializeField]
    private UnityEngine.Events.UnityEvent onInteraction;

    protected void Awake()
    {
        if (!gameObject.CompareTag("Interactive"))
        {
            Debug.LogWarningFormat("Object {0} contains interactive component, but isn't tagged properly", gameObject.name);
        }
    }

    public void Interact()
    {
        onInteraction.Invoke();
    }
}
