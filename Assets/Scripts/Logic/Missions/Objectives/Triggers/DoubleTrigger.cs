using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleTrigger : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.Events.UnityEvent firstTrigger;
    [SerializeField]
    private UnityEngine.Events.UnityEvent secondTrigger;

    [Header("Debug options")]
    [SerializeField]
    private bool printDebugMessages;

    protected void triggerFirstEvent()
    {
        firstTrigger.Invoke();
        if (printDebugMessages)
        {
            Debug.LogFormat("First trigger of {0} triggered", gameObject.name);
        }
    }

    protected void triggerSecondEvent()
    {
        secondTrigger.Invoke();
        if (printDebugMessages)
        {
            Debug.LogFormat("Second trigger of {0} triggered", gameObject.name);
        }
    }
}
