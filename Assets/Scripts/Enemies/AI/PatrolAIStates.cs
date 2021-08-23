using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class PatrolBaseState : BaseState
    {
        protected readonly AIPathNode pathNode;
        protected PatrolAIConfig config;

        public PatrolBaseState(AIPathNode node, PatrolAIConfig aiConfig) : base()
        {
            pathNode = node;
            config = aiConfig;
        }

        public override void Damaged(DamageInfo info)
        {
            base.Damaged(info);
            owner.ChangeState(new PatrolDamageCheckState(pathNode, config, info.direction));
            //Debug.Log("Look for damage source");
        }

        public override void Update()
        {
            base.Update();
            if (TargetVisible(owner.enemyController.target.layer) == TargetStatus.InSight)
            {
                Debug.DrawLine(owner.transform.position, owner.enemyController.target.transform.position, Color.red);
                owner.ChangeState(new ChaseState(pathNode, config));
            }
        }

        protected bool CheckForObstacle()
        {
            Vector3 position = owner.transform.position;
            Vector3 towardsTarget = pathNode.transform.position - position;
            const int layerMask = ~((1 << 7) | (1 << 9) | (1 << 12));
            return Physics.Raycast(position, towardsTarget, out RaycastHit raycastHit, towardsTarget.magnitude, layerMask);
        }

        protected void CheckForObstacleAndWaitIfNeeded()
        {
            if (CheckForObstacle())
            {
                //Debug.Log(string.Format("Obstacle on course of enemy {0}. Waiting", pathNode));
                owner.ChangeState(new WaitState(pathNode, config));
                return;
            }
        }

        protected void CheckForObstacleAndStopIfNeeded()
        {
            if (CheckForObstacle())
            {
                //Debug.Log(string.Format("Obstacle on course of enemy {0}. Stopping", pathNode));
                owner.ChangeState(new EmergencyStopState(pathNode, config));
                return;
            }
        }
    }

    public class RotateTowardsPointState : PatrolBaseState
    {
        public RotateTowardsPointState(AIPathNode node, PatrolAIConfig aiConfig) : base(node, aiConfig)
        {
        }

        public override void Enter()
        {
            base.Enter();
            movement.SetTargetRotation(Quaternion.LookRotation(pathNode.transform.position - owner.transform.position));
        }

        public override void Update()
        {
            if (movement.Velocity.magnitude > 0.1F)
            {
                owner.ChangeState(new EmergencyStopState(pathNode, config));
                return;
            }
            if (!movement.IsRotating)
            {
                CheckForObstacleAndWaitIfNeeded();
                owner.ChangeState(new AccelerateTowardsPointState(pathNode, config));
            }
            base.Update();
        }
    }

    public class AccelerateTowardsPointState : PatrolBaseState
    {
        public AccelerateTowardsPointState(AIPathNode node, PatrolAIConfig aiConfig) : base(node, aiConfig)
        {
        }

        public override void Enter()
        {
            base.Enter();
            float distanceToTarget = (pathNode.transform.position - owner.transform.position).magnitude;
            if (config.maxMovementSpeed * config.maxMovementSpeed / config.acceleration <= distanceToTarget)
            {
                config.actualMovementSpeed = config.maxMovementSpeed;
            }
            else
            {
                //actualMovementSpeed should be less than maxMovementSpeed
                config.actualMovementSpeed = Mathf.Sqrt(1.5F * distanceToTarget * config.acceleration);
            }
            //Debug.Log(string.Format("Actual speed set to {0}", config.actualMovementSpeed));
        }

        public override void PhysicsUpdate()
        {
            Vector3 position = owner.transform.position;
            Vector3 targetPosition = pathNode.transform.position;
            Vector3 towardsTarget = pathNode.transform.position - position;
            CheckForObstacleAndStopIfNeeded();
            if (movement.Velocity.magnitude < config.actualMovementSpeed)
            {
                if (Vector3.Angle(movement.Velocity, towardsTarget) > 5)
                {
                    //Not moving towards target
                    owner.ChangeState(new EmergencyStopState(pathNode, config));
                    return;
                }
                movement.MoveInGlobalCoordinatesIgnoringSpeed(towardsTarget.normalized * config.acceleration);
            }
            else
            {
                //Target speed reached
                StartGlide();
            }
            Debug.DrawLine(position, targetPosition, Color.cyan);
        }

        private void StartGlide()
        {
            //Debug.Log(string.Format("Accelaration towards point {0} finished. Starting glide", pathNode));
            movement.MoveRelativeToCamera(Vector3.zero);
            owner.ChangeState(new GlideTowardsPointState(pathNode, config));
        }
    }

    public class GlideTowardsPointState : PatrolBaseState
    {
        public GlideTowardsPointState(AIPathNode node, PatrolAIConfig aiConfig) : base(node, aiConfig)
        {
        }

        public override void PhysicsUpdate()
        {
            Vector3 position = owner.transform.position;
            Vector3 targetPosition = pathNode.transform.position;
            Vector3 towardsTarget = targetPosition - position;
            CheckForObstacleAndStopIfNeeded();
            if (movement.Velocity.magnitude < config.actualMovementSpeed)
            {
                //Too slow - speed up
                float speedDelta = config.actualMovementSpeed - movement.Velocity.magnitude;
                movement.MoveInGlobalCoordinatesIgnoringSpeed(towardsTarget.normalized * speedDelta);
            }
            //s = v0^2/a - 1/2*v0^2/a - point at which enemy has to start deceleration
            float stopDistance = config.actualMovementSpeed * config.actualMovementSpeed / config.acceleration - config.actualMovementSpeed * config.actualMovementSpeed / config.acceleration * 0.5F;
            if (towardsTarget.magnitude <= stopDistance)
            {
                StartDeceleration();
            }
            else
            {
                //Not moving towards target
                if (Vector3.Angle(movement.Velocity, towardsTarget) > 5)
                {
                    owner.ChangeState(new EmergencyStopState(pathNode, config));
                }
            }
            Debug.DrawLine(position, targetPosition, Color.cyan);
        }

        private void StartDeceleration()
        {
            //Debug.Log(string.Format("Gliding towards point {0} finished. Starting deceleration", pathNode));
            movement.MoveRelativeToCamera(Vector3.zero);
            owner.ChangeState(new DecelerateTowardsPointState(pathNode, config));
        }
    }

    public class DecelerateTowardsPointState : PatrolBaseState
    {
        public DecelerateTowardsPointState(AIPathNode node, PatrolAIConfig aiConfig) : base(node, aiConfig)
        {
        }

        public override void PhysicsUpdate()
        {
            Vector3 position = owner.transform.position;
            Vector3 targetPosition = pathNode.transform.position;
            Vector3 towardsTarget = targetPosition - position;
            CheckForObstacleAndStopIfNeeded();
            if (movement.Velocity.magnitude > config.speedEpsilon && Vector3.Angle(towardsTarget, movement.Velocity) <= 170)
            {
                if (Vector3.Angle(movement.Velocity, towardsTarget) > 5 && towardsTarget.magnitude > 1)
                {
                    //Not moving towards target
                    owner.ChangeState(new EmergencyStopState(pathNode, config));
                    return;
                }
                movement.MoveInGlobalCoordinatesIgnoringSpeed(-movement.Velocity.normalized * config.acceleration);
            }
            else
            {
                //Stopped or overshot
                movement.EnableStopMode();
                StartLookingAround();
            }
            Debug.DrawLine(position, targetPosition, Color.cyan);
        }

        private void StartLookingAround()
        {
            movement.MoveRelativeToCamera(Vector3.zero);
            movement.MoveInGlobalCoordinatesIgnoringSpeedAndTimeDelta(-movement.Velocity);
            //Debug.Log(string.Format("Deceleration towards point {0} finished. Starting looking around", pathNode));
            owner.ChangeState(new LookAroundPatrolState(pathNode, config));
        }
    }

    public class EmergencyStopState : PatrolBaseState
    {
        public EmergencyStopState(AIPathNode node, PatrolAIConfig aiConfig) : base(node, aiConfig)
        {
            //Debug.Log("Emergency stop");
        }

        public override void PhysicsUpdate()
        {
            if (movement.Velocity.magnitude > config.speedEpsilon)
            {
                movement.MoveInGlobalCoordinatesIgnoringSpeed(-movement.Velocity.normalized * config.acceleration);
            }
            else
            {
                //Stopped or overshot
                movement.MoveRelativeToCamera(Vector3.zero);
                movement.MoveInGlobalCoordinatesIgnoringSpeedAndTimeDelta(-movement.Velocity);
                movement.EnableStopMode();
                if (!movement.IsRotating)
                {
                    StartRotation();
                }
            }
        }

        private void StartRotation()
        {
            //Debug.Log(string.Format("Deceleration towards point {0} finished. Starting rotation", pathNode));
            owner.ChangeState(new RotateTowardsPointState(pathNode, config));
        }

        public override void Update()
        {
            //Not calling base, ignore player in sight
        }
    }

    public class WaitState : PatrolBaseState
    {
        public WaitState(AIPathNode node, PatrolAIConfig aiConfig) : base(node, aiConfig)
        {
        }

        public override void Update()
        {
            //Maybe move this stuff to a coroutine and execute less often
            if (!CheckForObstacle())
            {
                owner.ChangeState(new RotateTowardsPointState(pathNode, config));
                return;
            }
            base.Update();
        }
    }

    public class LookAroundPatrolState : PatrolBaseState
    {
        public LookAroundPatrolState(AIPathNode node, PatrolAIConfig aiConfig) : base(node, aiConfig)
        {
        }

        public override void Enter()
        {
            base.Enter();
            owner.enemyController.unitController.AnimationController.eventsAdapter.lookaroundEnd.AddListener(OnLookAroundEnd);
            owner.enemyController.unitController.AnimationController.ResetTriggers();
            owner.enemyController.unitController.AnimationController.SetState("LookAround");
        }

        private void OnLookAroundEnd()
        {
            StartRotation();
        }

        public override void Exit()
        {
            owner.enemyController.unitController.AnimationController.eventsAdapter.lookaroundEnd.RemoveListener(OnLookAroundEnd);
            base.Exit();
        }

        private void StartRotation()
        {
            //Debug.Log(string.Format("Finished looking around at point {0}. Starting roatation", pathNode));
            owner.ChangeState(new RotateTowardsPointState(pathNode.next, config));
        }
    }

    public class ChaseState : PatrolBaseState
    {
        private UnitShooting shooting;
        private AIShootingRules shootingRules;
        private AIShootingMode shootingMode;
        private AIShootingMode lastShootingMode;
        private bool lastShoot;
        private float phaseTime;
        private float lastSeenTime;

        public ChaseState(AIPathNode node, PatrolAIConfig aiConfig) : base(node, aiConfig)
        {
        }

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
            //Debug.Log(string.Format("Enemy {0} starting chase", owner.name));
            shooting = owner.enemyController.unitController.shooting;
            shootingRules = owner.enemyController.ShootingRules;
            lastShootingMode = AIShootingMode.NoShooting;
            phaseTime = 0;
            lastShoot = false;
            movement.UseDrag = true;
            owner.enemyController.unitController.AnimationController.ResetTriggers();
            owner.enemyController.unitController.AnimationController.SetState("Spotted");
            lastSeenTime = Time.time;
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            Vector3 position = owner.enemyController.transform.position;
            Vector3 targetPosition = owner.enemyController.target.transform.position;
            Vector3 positionDelta = targetPosition - position;
            switch (TargetVisible(owner.enemyController.target.layer))
            {
                case TargetStatus.InSight:
                    if (shooting.NeedsReload)
                    {
                        shooting.Reload();
                    }
                    if (positionDelta.magnitude > config.minDistance)
                    {
                        movement.MoveInGlobalCoordinates(positionDelta);
                    }
                    else
                    {
                        movement.MoveRelativeToCamera(Vector3.zero);
                    }
                    break;
                case TargetStatus.Covered:
                    movement.MoveRelativeToCamera(Vector3.zero);
                    break;
                case TargetStatus.TooFar:
                    movement.MoveInGlobalCoordinates(positionDelta);
                    break;
                default:
                    movement.MoveRelativeToCamera(Vector3.zero);
                    break;
            }
        }

        public override void Update()
        {
            Vector3 position = owner.enemyController.transform.position;
            Vector3 targetPosition = owner.enemyController.target.transform.position;
            Vector3 positionDelta = targetPosition - position;
            movement.SetRotationImmediately(positionDelta);
            switch (TargetVisible(owner.enemyController.target.layer))
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
                    lastSeenTime = Time.time;
                    break;
                case TargetStatus.Covered:
                    shooting.StopFire();
                    if (Time.time - lastSeenTime > config.chaseTimeout)
                    {
                        owner.ChangeState(new PatrolStopAndLookAroundState(pathNode, config));
                    }
                    break;
                case TargetStatus.TooFar:
                    shooting.StopFire();
                    if (Time.time - lastSeenTime > config.chaseTimeout)
                    {
                        owner.ChangeState(new PatrolStopAndLookAroundState(pathNode, config));
                    }
                    break;
            }
            Debug.DrawLine(position, targetPosition, Color.red);
        }

        public override void Exit()
        {
            base.Exit();
            movement.UseDrag = false;
        }

        public override void Damaged(DamageInfo info)
        {
            //Do not call base, ignore damage
        }
    }

    public class PatrolDamageCheckState : PatrolBaseState
    {
        private Vector3 hitDirection;
        private bool stopped;

        public PatrolDamageCheckState(AIPathNode node, PatrolAIConfig aiConfig, Vector3 _hitDirection) : base(node, aiConfig)
        {
            hitDirection = _hitDirection;
            stopped = false;
        }

        public override void Enter()
        {
            base.Enter();
            movement.SetTargetRotation(-hitDirection);
            owner.enemyController.unitController.AnimationController.ResetTriggers();
            owner.enemyController.unitController.AnimationController.SetState("LookAround");
        }

        public override void Update()
        {
            if (!movement.IsRotating)
            {
                //Debug.Log("Damage source not found, back to looking around");
                if (stopped)
                {
                    owner.ChangeState(new RotateTowardsPointState(pathNode, config));
                }
            }
            base.Update();
        }

        public override void PhysicsUpdate()
        {
            //Stopped or overshot
            if (movement.Velocity.magnitude <= config.speedEpsilon)
            {
                movement.MoveRelativeToCamera(Vector3.zero);
                movement.MoveInGlobalCoordinatesIgnoringSpeedAndTimeDelta(-movement.Velocity);
                stopped = true;
            }
            else
            {
                movement.MoveInGlobalCoordinatesIgnoringSpeed(-movement.Velocity.normalized * config.acceleration);
            }
        }
    }

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
            //Stopped or overshot
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
            //Debug.Log(string.Format("Finished looking around and stopping {0}. Starting roatation", pathNode));
            owner.ChangeState(new RotateTowardsPointState(pathNode, config));
        }
    }
}
