using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class PatrolAI : StateMachine
    {
        [SerializeField]
        private AIPathNode firstNode;
        [SerializeField]
        private float speedEpsilon;

        private void Awake()
        {
            ChangeState(new MoveToPointState(firstNode, speedEpsilon));
        }
    }
}
