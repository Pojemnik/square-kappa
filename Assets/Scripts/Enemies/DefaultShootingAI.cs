using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultShootingAI : AIStateMachine
{
    private void Awake()
    {
        ChangeState(new AIShootState());
    }
}
