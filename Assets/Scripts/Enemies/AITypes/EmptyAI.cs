using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class EmptyAI : StateMachine
    {
        private void Awake()
        {
            ChangeState(new BaseState());
        }
    }
}
