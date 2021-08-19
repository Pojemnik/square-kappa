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
            movement.SetLookTarget(positionDelta);
            if (TargetVisible(owner.enemyController.target.layer))
            {
                if (shooting.NeedsReload)
                {
                    shooting.Reload();
                }
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
                owner.ChangeState(new StaticLookAroundState(pathNode, config));
            }
        }
    }

    public class StaticLookAroundState : StaticBaseState
    {
        private Quaternion[] lookTargets;
        private Quaternion startDirection;
        private float startTime;
        private float duration;
        private int targetIndex;

        public StaticLookAroundState(AIPathNode node, StaticAIConfig aiConfig) : base(node, aiConfig)
        {
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
            ChangeLookTarget(lookTargets[targetIndex]);
        }

        public override void Update()
        {
            float t = (Time.time - startTime) / duration;
            Vector3 position = owner.transform.position;
            if (t >= 1)
            {
                if (++targetIndex == lookTargets.Length)
                {
                    targetIndex = 0;
                }
                ChangeLookTarget(lookTargets[targetIndex]);
            }
            else
            {
                movement.SetLookTarget(Quaternion.Slerp(startDirection, lookTargets[targetIndex], t));
            }
            if (TargetVisible(owner.enemyController.target.layer))
            {
                Debug.DrawLine(position, owner.enemyController.target.transform.position, Color.red);
                owner.ChangeState(new StaticShootState(pathNode, config));
            }
        }

        public override void Damaged(DamageInfo info)
        {
            base.Damaged(info);
            owner.ChangeState(new StaticDamageCheckState(pathNode, config, info.direction));
            //Debug.Log("Look for damage source");
        }

        private void ChangeLookTarget(Quaternion target)
        {
            startDirection = owner.transform.rotation * Quaternion.Euler(-90, 0, 0);
            startTime = Time.time;
            duration = Quaternion.Angle(target, startDirection) / config.rotationalSpeed;
            if (duration == 0)
            {
                duration = 1;
            }
        }
    }

    public class StaticDamageCheckState : StaticBaseState
    {
        private Quaternion startDirection;
        private Quaternion targetDirection;
        private float startTime;
        private float duration;
        private Vector3 hitDirection;

        public StaticDamageCheckState(AIPathNode node, StaticAIConfig aiConfig, Vector3 _hitDirection) : base(node, aiConfig)
        {
            hitDirection = _hitDirection;
        }

        private void ChangeLookTarget(Quaternion target)
        {
            startDirection = owner.transform.rotation * Quaternion.Euler(-90, 0, 0);
            startTime = Time.time;
            duration = Quaternion.Angle(target, startDirection) / config.rotationalSpeed;
            if (duration == 0)
            {
                duration = 1;
            }
        }

        public override void Enter()
        {
            base.Enter();
            targetDirection = Quaternion.LookRotation(-hitDirection);
            ChangeLookTarget(targetDirection);
        }

        public override void Update()
        {
            float t = (Time.time - startTime) / duration;
            Vector3 position = owner.transform.position;
            if (t >= 1)
            {
                //Debug.Log("Damage source not found, back to looking around");
                owner.ChangeState(new StaticLookAroundState(pathNode, config));
            }
            else
            {
                movement.SetLookTarget(Quaternion.Slerp(startDirection, targetDirection, t));
            }
            if (TargetVisible(owner.enemyController.target.layer))
            {
                Debug.DrawLine(position, owner.enemyController.target.transform.position, Color.red);
                owner.ChangeState(new StaticShootState(pathNode, config));
            }
        }
    }
}
