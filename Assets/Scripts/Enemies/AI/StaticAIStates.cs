using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class StaticBaseState : BaseState
    {
        protected readonly AIPathNode pathNode;
        protected StaticAIConfig config;

        public StaticBaseState(AIPathNode node, StaticAIConfig aiConfig) : base()
        {
            pathNode = node;
            config = aiConfig;
        }
    }

    public class StaticShootState : StaticBaseState
    {
        private UnitShooting shooting;
        private AIShootingRules shootingRules;
        private AIShootingMode shootingMode;
        private AIShootingMode lastShootingMode;
        private bool lastShoot;
        private float phaseTime;
        private float lastSeenTime;

        public StaticShootState(AIPathNode node, StaticAIConfig aiConfig) : base(node, aiConfig)
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
                        owner.ChangeState(new StaticLookAroundState(pathNode, config));
                    }
                    break;
                case TargetStatus.TooFar:
                    shooting.StopFire();
                    if (Time.time - lastSeenTime > config.chaseTimeout)
                    {
                        owner.ChangeState(new StaticLookAroundState(pathNode, config));
                    }
                    break;
            }
            Debug.DrawLine(position, targetPosition, Color.red);
        }
    }

    public class StaticLookAroundState : StaticBaseState
    {
        private Quaternion[] lookTargets;
        private int targetIndex;

        public StaticLookAroundState(AIPathNode node, StaticAIConfig aiConfig) : base(node, aiConfig)
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
            owner.enemyController.unitController.AnimationController.ResetTriggers();
            owner.enemyController.unitController.AnimationController.SetState("LookAround");
        }

        public override void Update()
        {
            if (TargetVisible(owner.enemyController.target.layer) == TargetStatus.InSight)
            {
                Debug.DrawLine(owner.transform.position, owner.enemyController.target.transform.position, Color.red);
                owner.ChangeState(new StaticShootState(pathNode, config));
            }
        }

        public override void Exit()
        {
            owner.enemyController.unitController.AnimationController.eventsAdapter.lookaroundEnd.RemoveListener(OnLookAroundEnd);
            base.Exit();
        }

        public override void Damaged(DamageInfo info)
        {
            base.Damaged(info);
            owner.ChangeState(new StaticDamageCheckState(pathNode, config, info.direction));
            //Debug.Log("Look for damage source");
        }
    }

    public class StaticDamageCheckState : StaticBaseState
    {
        private Vector3 hitDirection;

        public StaticDamageCheckState(AIPathNode node, StaticAIConfig aiConfig, Vector3 _hitDirection) : base(node, aiConfig)
        {
            hitDirection = _hitDirection;
        }

        public override void Enter()
        {
            base.Enter();
            movement.SetTargetRotation(-hitDirection);
        }

        public override void Update()
        {
            if (!movement.IsRotating)
            {
                //Debug.Log("Damage source not found, back to looking around");
                owner.ChangeState(new StaticLookAroundState(pathNode, config));
            }
            if (TargetVisible(owner.enemyController.target.layer) == TargetStatus.InSight)
            {
                Debug.DrawLine(owner.transform.position, owner.enemyController.target.transform.position, Color.red);
                owner.ChangeState(new StaticShootState(pathNode, config));
            }
        }
    }
}
