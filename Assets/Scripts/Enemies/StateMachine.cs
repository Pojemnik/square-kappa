using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class BaseState
    {
        public StateMachine owner;

        protected enum TargetStatus
        {
            InSight,
            Covered,
            Unavailable
        }

        protected TargetStatus TargetInSightCheck(int targetLayer)
        {
            Vector3 position = owner.enemyController.transform.position;
            Vector3 targetPosition = owner.enemyController.target.transform.position;
            RaycastHit raycastHit;
            Debug.DrawRay(position, targetPosition - position, Color.red);
            bool hit = Physics.Raycast(position, targetPosition - position, out raycastHit, owner.enemyController.VisionRange, owner.enemyController.layerMask);
            if (hit)
            {
                if (raycastHit.collider.gameObject.layer == targetLayer)
                {
                    return TargetStatus.InSight;
                }
                return TargetStatus.Covered;
            }
            else
            {
                return TargetStatus.Unavailable;
            }
        }

        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void Exit() { }
    }

    public class MoveTowardsTargetState : BaseState
    {
        private UnitMovement movement;

        public override void Enter()
        {
            movement = owner.enemyController.unitController.movement;
        }

        public override void Update()
        {
            Vector3 position = owner.enemyController.transform.position;
            Vector3 targetPosition = owner.enemyController.target.transform.position;
            Vector3 positionDelta = targetPosition - position;
            movement.SetLookTarget(positionDelta);
            switch (TargetInSightCheck(6))
            {
                case TargetStatus.InSight:
                    if (positionDelta.magnitude <= owner.enemyController.targetDistance)
                    {
                        movement.MoveRelative(Vector3.zero);
                    }
                    else if (positionDelta.magnitude <= owner.enemyController.VisionRange)
                    {
                        movement.MoveRelative(Vector3.forward);
                    }
                    else
                    {
                        movement.MoveRelative(Vector3.zero);
                    }
                    break;
                case TargetStatus.Covered:
                    //Avoid obstacle
                    break;
                case TargetStatus.Unavailable:
                    //Do nothing
                    break;
            }
        }
    }

    public class ShootState : BaseState
    {
        private UnitShooting shooting;
        private UnitMovement movement;
        private AIShootingRules shootingRules;
        private AIShootingMode shootingMode;
        private AIShootingMode lastShootingMode;
        private bool lastShoot;
        private float phaseTime;

        //TODO: Move constants to some scriptable object
        private void UpdateShooting()
        {
            AIShootingRulesInterpretation interpretation = owner.enemyController.ShootingRulesInterpretation;
            if (lastShootingMode != shootingMode)
            {
                lastShoot = false;
                phaseTime = 0;
            }
            phaseTime += Time.deltaTime;
            switch (shootingMode)
            {
                case AIShootingMode.Continous:
                    shooting.StartFire();
                    break;
                case AIShootingMode.Burst:
                    if (lastShoot)
                    {
                        if (phaseTime >= interpretation.BurstDuration)
                        {
                            shooting.StopFire();
                            phaseTime = 0;
                            lastShoot = false;
                        }
                    }
                    else
                    {
                        if (phaseTime >= interpretation.TimeBetweenBursts)
                        {
                            shooting.StartFire();
                            phaseTime = 0;
                            lastShoot = true;
                        }
                    }
                    break;
                case AIShootingMode.OneShot:
                    if (lastShoot)
                    {
                        lastShoot = false;
                        phaseTime = 0;
                        shooting.StopFire();
                    }
                    else
                    {
                        if (phaseTime >= interpretation.TimeBetweenShoots)
                        {
                            shooting.StartFire();
                            phaseTime = 0;
                            lastShoot = true;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public override void Enter()
        {
            shooting = owner.enemyController.unitController.shooting;
            movement = owner.enemyController.unitController.movement;
            shootingRules = owner.enemyController.ShootingRules;
            lastShootingMode = AIShootingMode.NoShooting;
            phaseTime = 0;
            lastShoot = false;
        }

        public override void Update()
        {
            Vector3 position = owner.enemyController.transform.position;
            Vector3 targetPosition = owner.enemyController.target.transform.position;
            Vector3 positionDelta = targetPosition - position;
            movement.SetLookTarget(positionDelta);
            switch (TargetInSightCheck(6))
            {
                case TargetStatus.InSight:
                    lastShootingMode = shootingMode;
                    shootingMode = AIShootingRuleCalculator.GetShootingMode(positionDelta.magnitude, shootingRules);
                    switch (shootingMode)
                    {
                        case AIShootingMode.NoShooting:
                            shooting.StopFire();
                            break;
                        case AIShootingMode.Error:
                            Debug.LogError("AI shooting mode error!");
                            break;
                        default:
                            UpdateShooting();
                            break;
                    }
                    break;
                case TargetStatus.Covered:
                    shooting.StopFire();
                    break;
                case TargetStatus.Unavailable:
                    shooting.StopFire();
                    break;
            }
        }
    }

    public class MoveToPointState : BaseState
    {
        private AIPathNode nextNode;
        private UnitMovement movement;
        private bool isMovingTowardsTarget;
        private readonly float speedEpsilon;

        public MoveToPointState(AIPathNode next, float eps)
        {
            nextNode = next;
            speedEpsilon = eps;
        }

        public override void Enter()
        {
            movement = owner.enemyController.unitController.movement;
            isMovingTowardsTarget = true;
        }

        public override void Update()
        {
            Vector3 position = owner.transform.position;
            Vector3 targetPosition = nextNode.transform.position;
            Debug.DrawLine(position, targetPosition, Color.cyan);
            Vector3 towardsTarget = targetPosition - position;
            if (isMovingTowardsTarget)
            {
                if (towardsTarget.magnitude < nextNode.epsilonRadius)
                {
                    Debug.Log(string.Format("Arrived at point {0}, stopping", nextNode));
                    movement.MoveRelative(Vector3.zero);
                    isMovingTowardsTarget = false;
                }
                else
                {
                    movement.SetLookTarget(towardsTarget);
                    movement.MoveRelative(Vector3.forward);
                }
            }
            else
            {
                if (movement.Velocity.magnitude > speedEpsilon)
                {
                    movement.SmoothLook(movement.Velocity, 180);
                    movement.MoveRelative(-Vector3.forward);
                }
                else
                {
                    Debug.Log(string.Format("Stopped at point {0}. Proceeding to next node...", nextNode));
                    owner.ChangeState(new MoveToPointState(nextNode.next, speedEpsilon));
                }
            }
        }
    }

    public class AvoidObstacleState : BaseState
    {
        //TODO: implement
    }

    public class StateMachine : MonoBehaviour
    {
        public EnemyController enemyController;

        private BaseState currentState;

        public void ChangeState(BaseState state)
        {
            if (currentState != null)
            {
                currentState.Exit();
            }
            currentState = state;
            currentState.owner = this;
            currentState.Enter();
        }

        private void Update()
        {
            currentState.Update();
        }
    }
}
