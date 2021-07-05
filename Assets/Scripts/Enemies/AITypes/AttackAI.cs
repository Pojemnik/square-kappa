using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class AttackAI : StateMachine
    {
        private void Awake()
        {
            ChangeState(new MoveTowardsTargetState());
        }
    }
}
