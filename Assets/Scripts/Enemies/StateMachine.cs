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
            Vector3 position = owner.transform.position;
            Vector3 targetPosition = owner.enemyController.target.transform.position;
            Vector3 towardsTarget = targetPosition - position;
            RaycastHit raycastHit;
            if (Physics.Raycast(position, targetPosition - position, out raycastHit, owner.enemyController.visibilitySphereRadius, owner.enemyController.layerMask))
            {
                if (raycastHit.collider.gameObject.layer == targetLayer)
                {
                    return true;
                }
            }
            if (Vector3.Angle(towardsTarget, owner.transform.up) <= owner.enemyController.visibilityConeAngle)
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
    }

    public class StaticShootState : BaseState
    {
        private UnitShooting shooting;
        private AIShootingRules shootingRules;
        private AIShootingMode shootingMode;
        private AIShootingMode lastShootingMode;
        private bool lastShoot;
        private float phaseTime;
        private readonly AIPathNode pathNode;
        private PatrolAIConfig config;

        public StaticShootState(AIPathNode node, PatrolAIConfig aIConfig)
        {
            pathNode = node;
            config = aIConfig;
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
            if (TargetVisible(owner.enemyController.target.layer))
            {
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
            }
            else
            {
                shooting.StopFire();
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
            duration = Quaternion.Angle(targetDirection, startDirection) / config.rotationalSpeed;
        }

        public override void Update()
        {
            if (movement.Velocity.magnitude > 0.1F)
            {
                owner.ChangeState(new EmergencyStopState(pathNode, config));
                return;
            }
            float t = (Time.time - startTime) / duration;
            Vector3 position = owner.transform.position;
            if (t >= 1)
            {
                Vector3 towardsTarget = pathNode.transform.position - position;
                const int layerMask = ~((1 << 7) | (1 << 9));
                if (Physics.Raycast(position, towardsTarget, out RaycastHit raycastHit, towardsTarget.magnitude, layerMask))
                {
                    //Obstacle
                    Debug.Log(string.Format("Obstacle on course of enemy {0}. Stopping", pathNode));
                    owner.ChangeState(new WaitState(pathNode, config));
                    return;
                }
                Debug.Log(string.Format("Finished rotating at point {0}. Starting movement", pathNode));
                owner.ChangeState(new AccelerateTowardsPointState(pathNode, config));
            }
            else
            {
                movement.SetLookTarget(Quaternion.Slerp(startDirection, targetDirection, t));
            }
            if (TargetVisible(owner.enemyController.target.layer))
            {
                Debug.DrawLine(position, owner.enemyController.target.transform.position, Color.red);
                owner.ChangeState(new ChaseState(pathNode, config));
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
            const int layerMask = ~((1 << 7) | (1 << 9));
            if (Physics.Raycast(position, towardsTarget, out RaycastHit raycastHit, towardsTarget.magnitude, layerMask))
            {
                //Obstacle
                Debug.Log(string.Format("Obstacle on course of enemy {0}. Stopping", pathNode));
                owner.ChangeState(new EmergencyStopState(pathNode, config));
                return;
            }
            if (movement.Velocity.magnitude < config.actualMovementSpeed)
            {
                movement.MoveInGlobalCoordinatesIgnoringSpeed(towardsTarget.normalized * config.acceleration);
                //Not moving towards target
                if (Vector3.Angle(movement.Velocity, towardsTarget) > 5)
                {
                    owner.ChangeState(new EmergencyStopState(pathNode, config));
                }
            }
            else
            {
                Debug.Log(string.Format("Accelaration towards point {0} finished. Starting glide", pathNode));
                movement.MoveRelativeToCamera(Vector3.zero);
                owner.ChangeState(new GlideTowardsPointState(pathNode, config));
            }
        }

        public override void Update()
        {
            base.Update();
            if (TargetVisible(owner.enemyController.target.layer))
            {
                Debug.DrawLine(owner.transform.position, owner.enemyController.target.transform.position, Color.red);
                owner.ChangeState(new ChaseState(pathNode, config));
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
            const int layerMask = ~((1 << 7) | (1 << 9));
            if (Physics.Raycast(position, towardsTarget, out RaycastHit raycastHit, towardsTarget.magnitude, layerMask))
            {
                //Obstacle
                Debug.Log(string.Format("Obstacle on course of enemy {0}. Stopping", pathNode));
                owner.ChangeState(new EmergencyStopState(pathNode, config));
                return;
            }
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
            else
            {
                //Not moving towards target
                if (Vector3.Angle(movement.Velocity, towardsTarget) > 5)
                {
                    owner.ChangeState(new EmergencyStopState(pathNode, config));
                }
            }
        }

        public override void Update()
        {
            base.Update();
            if (TargetVisible(owner.enemyController.target.layer))
            {
                Debug.DrawLine(owner.transform.position, owner.enemyController.target.transform.position, Color.red);
                owner.ChangeState(new ChaseState(pathNode, config));
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
            const int layerMask = ~((1 << 7) | (1 << 9));
            if (Physics.Raycast(position, towardsTarget, out RaycastHit raycastHit, towardsTarget.magnitude, layerMask))
            {
                //Obstacle
                owner.ChangeState(new EmergencyStopState(pathNode, config));
                return;
            }
            //Stopped or overshot
            if (movement.Velocity.magnitude <= config.speedEpsilon || Vector3.Angle(towardsTarget, movement.Velocity) > 170)
            {
                movement.MoveRelativeToCamera(Vector3.zero);
                movement.MoveInGlobalCoordinatesIgnoringSpeedAndTimeDelta(-movement.Velocity);
                Debug.Log(string.Format("Deceleration towards point {0} finished. Starting looking around", pathNode));
                owner.ChangeState(new LookAroundState(pathNode, config));
            }
            else
            {
                movement.MoveInGlobalCoordinatesIgnoringSpeed(-movement.Velocity.normalized * config.acceleration);
                //Not moving towards target
                if (Vector3.Angle(movement.Velocity, towardsTarget) > 5 && towardsTarget.magnitude > 1)
                {
                    owner.ChangeState(new EmergencyStopState(pathNode, config));
                }
            }
        }

        public override void Update()
        {
            base.Update();
            if (TargetVisible(owner.enemyController.target.layer))
            {
                Debug.DrawLine(owner.transform.position, owner.enemyController.target.transform.position, Color.red);
            }
        }
    }

    public class EmergencyStopState : BaseState
    {
        private readonly AIPathNode pathNode;
        private readonly PatrolAIConfig config;

        public EmergencyStopState(AIPathNode node, PatrolAIConfig aIConfig)
        {
            pathNode = node;
            config = aIConfig;
            Debug.Log("Emergency stop");
        }

        public override void PhysicsUpdate()
        {
            //Stopped or overshot
            if (movement.Velocity.magnitude <= config.speedEpsilon)
            {
                movement.MoveRelativeToCamera(Vector3.zero);
                movement.MoveInGlobalCoordinatesIgnoringSpeedAndTimeDelta(-movement.Velocity);
                if (!movement.IsRotating())
                {
                    Debug.Log(string.Format("Deceleration towards point {0} finished. Starting rotation", pathNode));
                    owner.ChangeState(new RotateTowardsPointState(pathNode, config));
                }
            }
            else
            {
                movement.MoveInGlobalCoordinatesIgnoringSpeed(-movement.Velocity.normalized * config.acceleration);
            }
        }
    }

    public class WaitState : BaseState
    {
        private readonly AIPathNode pathNode;
        private PatrolAIConfig config;

        public WaitState(AIPathNode node, PatrolAIConfig aIConfig)
        {
            pathNode = node;
            config = aIConfig;
        }

        public override void Update()
        {
            //Maybe move this stuff to a coroutine and execute less often
            Vector3 position = owner.transform.position;
            Vector3 targetPosition = pathNode.transform.position;
            Vector3 towardsTarget = targetPosition - position;
            const int layerMask = ~((1 << 7) | (1 << 9));
            if (!Physics.Raycast(position, towardsTarget, out RaycastHit raycastHit, towardsTarget.magnitude, layerMask))
            {
                //No obstacle
                owner.ChangeState(new RotateTowardsPointState(pathNode, config));
                return;
            }
            if (TargetVisible(owner.enemyController.target.layer))
            {
                Debug.DrawLine(position, owner.enemyController.target.transform.position, Color.red);
                owner.ChangeState(new ChaseState(pathNode, config));
            }
        }
    }

    public class LookAroundState : BaseState
    {
        private readonly AIPathNode pathNode;
        private PatrolAIConfig config;
        private Quaternion[] lookTargets;
        private Quaternion startDirection;
        private float startTime;
        private float duration;
        private int targetIndex;

        public LookAroundState(AIPathNode node, PatrolAIConfig aIConfig)
        {
            pathNode = node;
            config = aIConfig;
        }

        public override void Enter()
        {
            base.Enter();
            targetIndex = 0;
            lookTargets = new Quaternion[config.lookAroundRotations.Count];
            for (int i = 0; i < lookTargets.Length; i++)
            {
                lookTargets[i] = owner.transform.rotation * Quaternion.Euler(config.lookAroundRotations[i]);
            }
            ChangeLookTarget(targetIndex);
        }

        public override void Update()
        {
            float t = (Time.time - startTime) / duration;
            Vector3 position = owner.transform.position;
            if (t >= 1)
            {
                if (++targetIndex == lookTargets.Length)
                {
                    owner.ChangeState(new RotateTowardsPointState(pathNode.next, config));
                    Debug.Log(string.Format("Finished looking around at point {0}. Starting roatation", pathNode));
                    return;
                }
                else
                {
                    ChangeLookTarget(targetIndex);
                }
            }
            else
            {
                movement.SetLookTarget(Quaternion.Slerp(startDirection, lookTargets[targetIndex], t));
            }
            if (TargetVisible(owner.enemyController.target.layer))
            {
                Debug.DrawLine(position, owner.enemyController.target.transform.position, Color.red);
                owner.ChangeState(new ChaseState(pathNode, config));
            }
        }

        private void ChangeLookTarget(int index)
        {
            startDirection = owner.transform.rotation * Quaternion.Euler(-90, 0, 0);
            startTime = Time.time;
            duration = Quaternion.Angle(lookTargets[index], startDirection) / config.rotationalSpeed;
        }
    }

    public class ChaseState : BaseState
    {
        private UnitShooting shooting;
        private AIShootingRules shootingRules;
        private AIShootingMode shootingMode;
        private AIShootingMode lastShootingMode;
        private bool lastShoot;
        private float phaseTime;
        private readonly AIPathNode pathNode;
        private PatrolAIConfig config;

        public ChaseState(AIPathNode node, PatrolAIConfig aIConfig)
        {
            pathNode = node;
            config = aIConfig;
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
            Debug.Log(string.Format("Enemy {0} starting chase", owner.name));
            shooting = owner.enemyController.unitController.shooting;
            shootingRules = owner.enemyController.ShootingRules;
            lastShootingMode = AIShootingMode.NoShooting;
            phaseTime = 0;
            lastShoot = false;
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            Vector3 position = owner.enemyController.transform.position;
            Vector3 targetPosition = owner.enemyController.target.transform.position;
            Vector3 positionDelta = targetPosition - position;
            if (TargetVisible(owner.enemyController.target.layer))
            {
                movement.MoveInGlobalCoordinates(positionDelta);
            }
            else
            {
                owner.ChangeState(new EmergencyStopState(pathNode, config));
                return;
            }
        }

        public override void Update()
        {
            Vector3 position = owner.enemyController.transform.position;
            Vector3 targetPosition = owner.enemyController.target.transform.position;
            Vector3 positionDelta = targetPosition - position;
            movement.SetLookTarget(positionDelta);
            if (TargetVisible(owner.enemyController.target.layer))
            {
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
            }
            else
            {
                shooting.StopFire();
                owner.ChangeState(new RotateTowardsPointState(pathNode.next, config));
            }
        }
    }

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

        private void Awake()
        {
            states = new Stack<BaseState>();
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
