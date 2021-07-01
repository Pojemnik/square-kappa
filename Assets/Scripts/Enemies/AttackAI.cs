using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAI : AIStateMachine
{
    private void Awake()
    {
        ChangeState(new AIMoveTowardsTargetState());
    }
}
