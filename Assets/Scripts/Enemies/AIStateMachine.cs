using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBaseState
{
    public AIStateMachine owner;

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

public class AIMoveTowardsTargetState : AIBaseState
{
    private UnitMovement movement;

    public override void Enter()
    {
        movement = owner.enemyController.unitController.movement;
        Debug.Log("Entering move state");
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
                    movement.MoveRelative(new Vector3(0, 0, 1));
                }
                else
                {
                    movement.MoveRelative(Vector3.zero);
                    Debug.Log("Too far to see");
                }
                break;
            case TargetStatus.Covered:
                //Avoid obstacle
                Debug.Log("Covered by an object");
                break;
            case TargetStatus.Unavailable:
                //Do nothing
                break;
        }
    }
}

public class AIShootState : AIBaseState
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
        if (lastShootingMode != shootingMode)
        {
            lastShoot = false;
            phaseTime = 0;
            Debug.Log(string.Format("New shooting mode: {0}", shootingMode));
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
                    if (phaseTime >= 2)
                    {
                        shooting.StopFire();
                        phaseTime = 0;
                        lastShoot = false;
                    }
                }
                else
                {
                    if (phaseTime >= 2)
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
                    if (phaseTime >= 2)
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

public class AIAvoidObstacleState : AIBaseState
{
    //TODO: implement
}

public class AIStateMachine : MonoBehaviour
{
    public EnemyController enemyController;

    private AIBaseState currentState;

    public void ChangeState(AIBaseState state)
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
