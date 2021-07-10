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

        public virtual void Enter()
        {
            movement = owner.enemyController.unitController.movement;
        }
        public virtual void Update() { }
        public virtual void PhysicsUpdate() { }
        public virtual void Exit() { }
    }

    public class MoveTowardsTargetState : BaseState
    {
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
                        movement.MoveRelativeToCamera(Vector3.zero);
                    }
                    else if (positionDelta.magnitude <= owner.enemyController.VisionRange)
                    {
                        movement.MoveRelativeToCamera(Vector3.forward);
                    }
                    else
                    {
                        movement.MoveRelativeToCamera(Vector3.zero);
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
            base.Enter();
            shooting = owner.enemyController.unitController.shooting;
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
        private readonly AIPathNode pathNode;
        private readonly PatrolAIConfig config;

        public MoveToPointState(AIPathNode node, PatrolAIConfig aIConfig)
        {
            pathNode = node;
            config = aIConfig;
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Update()
        {
            Vector3 position = owner.transform.position;
            Vector3 targetPosition = pathNode.transform.position;
            Debug.DrawLine(position, targetPosition, Color.cyan);
            Vector3 towardsTarget = targetPosition - position;
            if (towardsTarget.magnitude < pathNode.epsilonRadius)
            {
                Debug.Log(string.Format("Arrived at point {0}, stopping", pathNode));
                movement.MoveRelativeToCamera(Vector3.zero);
                owner.ChangeState(new StopAtPointState(pathNode, config));
            }
            else
            {
                movement.MoveInGlobalCoordinates(towardsTarget);
            }
        }
    }

    public class StopAtPointState : BaseState
    {
        private readonly AIPathNode pathNode;
        private readonly PatrolAIConfig config;

        public StopAtPointState(AIPathNode node, PatrolAIConfig aIConfig)
        {
            pathNode = node;
            config = aIConfig;
        }

        public override void Update()
        {
            Vector3 position = owner.transform.position;
            Vector3 targetPosition = pathNode.transform.position;
            Debug.DrawLine(position, targetPosition, Color.cyan);
            if (movement.Velocity.magnitude > config.speedEpsilon)
            {
                movement.MoveInGlobalCoordinates(-movement.Velocity);
            }
            else
            {
                Debug.Log(string.Format("Stopped at point {0}. Starting rotation", pathNode));
                movement.MoveRelativeToCamera(Vector3.zero);
                owner.ChangeState(new RotateTowardsPointState(pathNode.next, config));
            }
        }
    }

    public class RotateTowardsPointState : BaseState
    {
        private readonly AIPathNode pathNode;
        private readonly PatrolAIConfig config;
        private Quaternion startDirection;
        private Quaternion targetDirection;
        private float startTime;
        private float duration;

        public RotateTowardsPointState(AIPathNode node, PatrolAIConfig aIConfig)
        {
            pathNode = node;
            config = aIConfig;
        }

        public override void Enter()
        {
            base.Enter();
            Vector3 position = owner.transform.position;
            Vector3 targetPosition = pathNode.transform.position;
            targetDirection = Quaternion.LookRotation(targetPosition - position);
            startDirection = owner.transform.rotation * Quaternion.Euler(-90, 0, 0);
            startTime = Time.time;
            duration = Vector3.Angle(targetDirection.eulerAngles, startDirection.eulerAngles) / config.rotationalSpeed;
        }

        public override void Update()
        {
            float t = (Time.time - startTime) / duration;
            if (t >= 1)
            {
                Debug.Log(string.Format("Finished rotating at point {0}. Starting movement", pathNode));
                owner.ChangeState(new AccelerateTowardsPointState(pathNode, config));
            }
            else
            {
                movement.SetLookTarget(Quaternion.Slerp(startDirection, targetDirection, t));
            }
        }
    }

    public class AccelerateTowardsPointState : BaseState
    {
        private readonly AIPathNode pathNode;
        private PatrolAIConfig config;

        public AccelerateTowardsPointState(AIPathNode node, PatrolAIConfig aIConfig)
        {
            pathNode = node;
            config = aIConfig;
        }

        public override void Enter()
        {
            base.Enter();
            Vector3 towardsTarget = pathNode.transform.position - owner.transform.position;
            if (config.maxMovementSpeed * config.maxMovementSpeed / config.acceleration > towardsTarget.magnitude)
            {
                //actual speed != max speed
                config.actualMovementSpeed = Mathf.Sqrt(1.5F * towardsTarget.magnitude * config.acceleration);
            }
            else
            {
                config.actualMovementSpeed = config.maxMovementSpeed;
            }
            Debug.Log(string.Format("Actual speed set to {0}", config.actualMovementSpeed));
        }

        public override void PhysicsUpdate()
        {
            Vector3 position = owner.transform.position;
            Vector3 targetPosition = pathNode.transform.position;
            Debug.DrawLine(position, targetPosition, Color.cyan);
            Vector3 towardsTarget = targetPosition - position;
            if (movement.Velocity.magnitude < config.actualMovementSpeed)
            {
                movement.MoveInGlobalCoordinatesIgnoringSpeed(towardsTarget.normalized * config.acceleration);
            }
            else
            {
                Debug.Log(string.Format("Accelaration towards point {0} finished. Starting glide", pathNode));
                movement.MoveRelativeToCamera(Vector3.zero);
                owner.ChangeState(new GlideTowardsPointState(pathNode, config));
            }
        }
    }

    public class GlideTowardsPointState : BaseState
    {
        private readonly AIPathNode pathNode;
        private readonly PatrolAIConfig config;

        public GlideTowardsPointState(AIPathNode node, PatrolAIConfig aIConfig)
        {
            pathNode = node;
            config = aIConfig;
        }

        public override void PhysicsUpdate()
        {
            Vector3 position = owner.transform.position;
            Vector3 targetPosition = pathNode.transform.position;
            Debug.DrawLine(position, targetPosition, Color.cyan);
            Vector3 towardsTarget = targetPosition - position;
            if (movement.Velocity.magnitude < config.actualMovementSpeed)
            {
                movement.MoveInGlobalCoordinatesIgnoringSpeed(towardsTarget.normalized * (config.actualMovementSpeed - movement.Velocity.magnitude));
            }
            //s = v0^2/a - 1/2*v0^2/a - point at which enemy has to start deceleration
            if (towardsTarget.magnitude <= config.actualMovementSpeed * config.actualMovementSpeed / config.acceleration - config.actualMovementSpeed * config.actualMovementSpeed / config.acceleration * 0.5F)
            {
                Debug.Log(string.Format("Gliding towards point {0} finished. Starting deceleration", pathNode));
                movement.MoveRelativeToCamera(Vector3.zero);
                owner.ChangeState(new DecelerateTowardsPointState(pathNode, config));
            }
        }
    }

    public class DecelerateTowardsPointState : BaseState
    {
        private readonly AIPathNode pathNode;
        private readonly PatrolAIConfig config;

        public DecelerateTowardsPointState(AIPathNode node, PatrolAIConfig aIConfig)
        {
            pathNode = node;
            config = aIConfig;
        }

        public override void PhysicsUpdate()
        {
            Vector3 position = owner.transform.position;
            Vector3 targetPosition = pathNode.transform.position;
            Debug.DrawLine(position, targetPosition, Color.cyan);
            Vector3 towardsTarget = targetPosition - position;
            //Debug.Log(Vector3.Angle(towardsTarget, movement.Velocity));
            //Stopped or overshot
            if (movement.Velocity.magnitude <= config.speedEpsilon || Vector3.Angle(towardsTarget, movement.Velocity) > 170)
            {
                movement.MoveRelativeToCamera(Vector3.zero);
                movement.MoveInGlobalCoordinatesIgnoringSpeedAndTimeDelta(-movement.Velocity);
                Debug.Log(string.Format("Deceleration towards point {0} finished. Starting rotation", pathNode));
                owner.ChangeState(new RotateTowardsPointState(pathNode.next, config));
            }
            else
            {
                movement.MoveInGlobalCoordinatesIgnoringSpeed(-movement.Velocity.normalized * config.acceleration);
            }
        }
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

        private void FixedUpdate()
        {
            currentState.PhysicsUpdate();
        }
    }
}
