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

public class AIMoveState : AIBaseState
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
                if (positionDelta.magnitude < owner.enemyController.ShootingRange)
                {
                    owner.ChangeState(new AIShootState());
                }
                else if(positionDelta.magnitude < owner.enemyController.VisionRange)
                { 
                    movement.MoveRelative(new Vector3(0, 0, 1));
                    Debug.Log("Moving towards target");
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
                Debug.Log("Target unavailable");
                break;
        }
    }
}

public class AIShootState : AIBaseState
{
    private UnitShooting shooting;
    private UnitMovement movement;

    public override void Enter()
    {
        shooting = owner.enemyController.unitController.shooting;
        movement = owner.enemyController.unitController.movement;
        Debug.Log("Entering shoot state");
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
                if (positionDelta.magnitude < owner.enemyController.ShootingRange)
                {
                    shooting.StartFire();
                }
                else
                {
                    shooting.StopFire();
                    owner.ChangeState(new AIMoveState());
                }
                break;
            case TargetStatus.Covered:
                owner.ChangeState(new AIMoveState());
                break;
            case TargetStatus.Unavailable:
                //Do nothing
                break;
        }
    }
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

    private void Start()
    {
        ChangeState(new AIMoveState());
    }

    private void Update()
    {
        currentState.Update();
    }
}
