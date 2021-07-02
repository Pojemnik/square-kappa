using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAI : AIStateMachine
{
    [SerializeField]
    private AIPathNode firstNode;

    private void Awake()
    {
        ChangeState(new AIMoveToPointState(firstNode));
    }
}
