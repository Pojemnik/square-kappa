using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    [RequireComponent(typeof(Rigidbody))]
    public class PatrolAI : StateMachine
    {
        [SerializeField]
        private AIPathNode firstNode;
        [SerializeField]
        private PatrolAIConfig config;

        private void Start()
        {
            if(config == null)
            {
                throw new System.Exception(string.Format("No PatrolAIConfig in object {0}", name));
            }
            GetComponent<UnitMovement>().UseDrag = false;
            ChangeState(new RotateTowardsPointState(firstNode, config));
        }
    }
}
