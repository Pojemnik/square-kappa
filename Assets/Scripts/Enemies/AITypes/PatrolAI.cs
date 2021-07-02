using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAI : AIStateMachine
{
    [SerializeField]
    private AIPathNode firstNode;
    [SerializeField]
    private float speedEpsilon;

    private void Awake()
    {
        ChangeState(new AIMoveToPointState(firstNode, speedEpsilon));
    }
}
