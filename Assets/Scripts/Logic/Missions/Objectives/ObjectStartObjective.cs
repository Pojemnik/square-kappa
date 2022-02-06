using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStartObjective : Objective
{
    private void Start()
    {
        if (MissionsManager.Instance.initFinished)
        {
            ChangeState();
        }
        else
        {
            MissionsManager.Instance.initFinishedEvent += (s, a) => ChangeState();
        }
    }

    private void ChangeState()
    {
        if (defaultState)
        {
            Uncomplete();
        }
        else
        {
            Complete();
        }
    }
}
