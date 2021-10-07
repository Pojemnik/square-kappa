using UnityEngine;

namespace AI
{
    public class PatrolStopAndLookAroundState : PatrolBaseState
    {
        private bool stopped;
        private bool animationFinished;

        public PatrolStopAndLookAroundState(AIPathNode node, PatrolAIConfig aiConfig) : base(node, aiConfig)
        {
            stopped = false;
        }

        public override void Enter()
        {
            base.Enter();
            owner.enemyController.unitController.AnimationController.eventsAdapter.lookaroundEnd.AddListener(OnLookAroundEnd);
            owner.enemyController.unitController.AnimationController.ResetTriggers();
            owner.enemyController.unitController.AnimationController.SetState("LookAround");
            owner.status = "Looking for lost target";
        }

        public override void Update()
        {
            if (TargetVisible(owner.enemyController.target.layer) == TargetStatus.InSight)
            {
                Debug.DrawLine(owner.transform.position, owner.enemyController.target.transform.position, Color.red);
                owner.ChangeState(new ChaseState(pathNode, config));
            }
        }

        private void OnLookAroundEnd()
        {
            animationFinished = true;
            if (stopped)
            {
                StartRotation();
            }
        }

        public override void Exit()
        {
            owner.enemyController.unitController.AnimationController.eventsAdapter.lookaroundEnd.RemoveListener(OnLookAroundEnd);
            base.Exit();
        }

        public override void PhysicsUpdate()
        {
            if (movement.Velocity.magnitude <= config.speedEpsilon)
            {
                movement.MoveRelativeToCamera(Vector3.zero);
                movement.MoveInGlobalCoordinatesIgnoringSpeedAndTimeDelta(-movement.Velocity);
                stopped = true;
                if (animationFinished)
                {
                    StartRotation();
                }
            }
            else
            {
                movement.MoveInGlobalCoordinatesIgnoringSpeed(-movement.Velocity.normalized * config.acceleration);
            }
        }

        private void StartRotation()
        {
            owner.ChangeState(new RotateTowardsPointState(pathNode, config));
        }
    }
}
