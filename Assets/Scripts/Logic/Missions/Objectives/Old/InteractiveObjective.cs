using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObjective : Objective, IInteractive
{
    private bool completed;

    protected override void Awake()
    {
        base.Awake();
        completed = defaultState;
        if(!gameObject.CompareTag("Interactive"))
        {
            Debug.LogWarningFormat("Object {0} contains interactive component, but isn't tagged properly", gameObject.name);
        }
    }

    public void Interact()
    {
        if(completed)
        {
            Uncomplete();
            completed = false;
        }
        else
        {
            Complete();
            completed = true;
        }
    }
}
