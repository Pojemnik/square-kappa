using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.Events.UnityEvent onTrigger;

    [Header("Debug options")]
    [SerializeField]
    private bool printDebugMessages;

    protected void triggerEvent()
    {
        onTrigger.Invoke();
        if(printDebugMessages)
        {
            Debug.LogFormat("Trigger {0} triggered", gameObject.name);
        }
    }
}
