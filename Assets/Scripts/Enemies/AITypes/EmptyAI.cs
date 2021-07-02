using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyAI : AIStateMachine
{
    private void Awake()
    {
        ChangeState(new AIBaseState());
    }
}
