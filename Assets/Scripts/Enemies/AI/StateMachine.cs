using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class BaseState
    {
        public StateMachine owner;
        protected UnitMovement movement;

        protected enum TargetStatus
        {
            InSight,
            Covered,
            Unavailable
        }

        protected bool TargetVisible(int targetLayer)
        {
            Vector3 position = owner.enemyController.head.transform.position;
            Vector3 targetPosition = owner.enemyController.target.transform.position;
            Vector3 towardsTarget = targetPosition - position;
            Debug.DrawRay(position, towardsTarget, Color.magenta);
            if (Physics.Raycast(position, towardsTarget, out RaycastHit raycastHit, owner.enemyController.visibilitySphereRadius, owner.enemyController.layerMask))
            {
                if (raycastHit.collider.gameObject.layer == targetLayer)
                {
                    return true;
                }
            }
            if (Vector3.Angle(towardsTarget, owner.enemyController.head.transform.up) <= owner.enemyController.visibilityConeAngle)
            {
                if (Physics.Raycast(position, targetPosition - position, out raycastHit, owner.enemyController.visibilityConeHeight, owner.enemyController.layerMask))
                {
                    if (raycastHit.collider.gameObject.layer == targetLayer)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual void Enter()
        {
            movement = owner.enemyController.unitController.movement;
        }

        public virtual void Update() { }
        public virtual void PhysicsUpdate() { }
        public virtual void Exit() { }
        public virtual void Damaged(DamageInfo info) { }
    }
    
    [RequireComponent(typeof(Health))]
    public class StateMachine : MonoBehaviour
    {
        public EnemyController enemyController;

        private BaseState currentState;
        private Stack<BaseState> states;

        public void ChangeState(BaseState state)
        {
            if (states.Count > 0)
            {
                BaseState lastTop = states.Pop();
                if (lastTop != null)
                {
                    lastTop.Exit();
                }
            }
            states.Push(state);
            currentState = state;
            currentState.owner = this;
            currentState.Enter();
        }

        public void PushState(BaseState state)
        {
            if (states.Count > 0)
            {
                BaseState lastTop = states.Peek();
                if (lastTop != null)
                {
                    lastTop.Exit();
                }
            }
            states.Push(state);
            currentState = state;
            currentState.owner = this;
            currentState.Enter();
        }

        public void PopState()
        {
            if (states.Count <= 1)
            {
                throw new System.Exception(string.Format("Critical error. State stack reached bottom at enemy {0}", name));
            }
            BaseState state = states.Pop();
            if (state != null)
            {
                state.Exit();
            }
            currentState = states.Peek();
            if (currentState != null)
            {
                currentState.owner = this;
                currentState.Enter();
            }
        }

        private void OnDamage(DamageInfo info)
        {
            currentState.Damaged(info);
        }

        private void Awake()
        {
            states = new Stack<BaseState>();
            GetComponent<Health>().damageEvent.AddListener(OnDamage);
        }

        private void Update()
        {
            currentState.Update();
        }

        private void FixedUpdate()
        {
            currentState.PhysicsUpdate();
        }
    }
}
