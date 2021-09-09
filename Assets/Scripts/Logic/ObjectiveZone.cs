using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ObjectiveZone : Objective
{
    [SerializeField]
    private bool completedWhenLeft;
    [Header("Debug options")]
    [SerializeField]
    private bool displayDebugInfo;

    private int triggersCount;
    private int currentlyCheckedTriggers;

    protected override void Awake()
    {
        base.Awake();
        triggersCount = GetComponents<Collider>().Length;
        currentlyCheckedTriggers = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerTrigger"))
        {
            currentlyCheckedTriggers++;
            if (displayDebugInfo)
            {
                Debug.LogFormat("Objective zone {0} entered", gameObject.name);
                Debug.LogFormat("Checked triggers count: {0}", currentlyCheckedTriggers);
            }
            if (currentlyCheckedTriggers == 1)
            {
                if (completedWhenLeft)
                {
                    Uncomplete();
                }
                else
                {
                    Complete();
                }

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerTrigger"))
        {
            currentlyCheckedTriggers--;
            if (displayDebugInfo)
            {
                Debug.LogFormat("Objective zone {0} left", gameObject.name);
                Debug.LogFormat("Checked triggers count: {0}", currentlyCheckedTriggers);
            }
            if (currentlyCheckedTriggers == 0)
            {
                if (completedWhenLeft)
                {
                    Complete();
                }
                else
                {
                    Uncomplete();
                }
            }
        }
    }
}
