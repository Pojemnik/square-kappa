using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ZoneTrigger : DoubleTrigger
{
    private int currentlyCheckedTriggers;

    private void Awake()
    {
        currentlyCheckedTriggers = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerTrigger"))
        {
            currentlyCheckedTriggers++;
            if (currentlyCheckedTriggers == 1)
            {
                triggerFirstEvent();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerTrigger"))
        {
            currentlyCheckedTriggers--;
            if (currentlyCheckedTriggers == 0)
            {
                triggerSecondEvent();
            }
        }
    }
}
