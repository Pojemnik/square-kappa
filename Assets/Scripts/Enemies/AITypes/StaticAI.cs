using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    [RequireComponent(typeof(Rigidbody))]
    public class StaticAI : StateMachine
    {
        [SerializeField]
        private AIPathNode firstNode;
        [SerializeField]
        private StaticAIConfig config;

        private void Start()
        {
            if (config == null)
            {
                throw new System.Exception(string.Format("No PatrolAIConfig in object {0}", name));
            }
            GetComponent<Rigidbody>().drag = 0;
            ChangeState(new StaticLookAroundState(firstNode, config));
        }
    }
}
