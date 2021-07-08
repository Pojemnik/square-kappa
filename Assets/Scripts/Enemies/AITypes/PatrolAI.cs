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
        private PatrolAIConfig config;

        private void Awake()
        {
            if(config == null)
            {
                throw new System.Exception(string.Format("No PatrolAIConfig in object {0}", name));
            }
            ChangeState(new RotateTowardsPointState(firstNode, config));
        }
    }
}
