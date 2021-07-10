using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class DefaultShootingAI : StateMachine
    {
        private void Start()
        {
            ChangeState(new ShootState());
        }
    }
}
