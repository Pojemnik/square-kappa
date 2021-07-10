using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class AttackAI : StateMachine
    {
        private void Start()
        {
            ChangeState(new MoveTowardsTargetState());
        }
    }
}
