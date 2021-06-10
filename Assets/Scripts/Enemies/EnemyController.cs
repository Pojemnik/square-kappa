using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAIState
{
    void Enter(PlayerController unitController);
    void Update(PlayerController unitController);
    void Exit(PlayerController unitController);
}

public class AIEmptyState : IAIState
{
    public void Enter(PlayerController unitController) { }
    public void Exit(PlayerController unitController) { }
    public void Update(PlayerController unitController) { }
}

public class AIMovingState : AIEmptyState
{
    private Rigidbody rigidbody;

    public new void Enter(PlayerController unitController)
    {
        rigidbody = unitController.gameObject.GetComponent<Rigidbody>();
    }

    public new void Update(PlayerController unitController)
    {
        //Vector3 positionDelta = target.transform.position - unitController.gameObject.rigidbody.position;
        //unitController.SetLookTarget(positionDelta);
        //if (positionDelta.magnitude > visionRange)
        //{
        //    print("Too far to see");
        //    return;
        //}
    }
}

public class AIStateMachine
{

}

public class EnemyController : MonoBehaviour
{
    public GameObject player;
    public float shootingRange;
    public float visionRange;

    private PlayerController unitController;
    private new Rigidbody rigidbody;
    private int layerMask;

    public void OnDeath()
    {
        Destroy(gameObject);
    }

    void Start()
    {
        unitController = GetComponent<PlayerController>();
        unitController.movement.cameraAiming = false;
        rigidbody = GetComponent<Rigidbody>();
        layerMask = (1 << 7) | (1 << 8) | (1 << 9);
        layerMask = ~layerMask;
    }

    void Update()
    {
        Vector3 positionDelta = player.transform.position - rigidbody.position;
        unitController.movement.SetLookTarget(positionDelta);
        if (positionDelta.magnitude > visionRange)
        {
            print("Too far to see");
            return;
        }
        RaycastHit raycastHit;
        if (Physics.Raycast(transform.position, positionDelta, out raycastHit, visionRange, layerMask))
        {
            //Player hit
            if (raycastHit.transform.gameObject.layer == 6)
            {
                if (positionDelta.magnitude < shootingRange)
                {
                    unitController.shooting.StartFire();
                    unitController.movement.MoveRelative(Vector3.zero);
                }
                else
                {
                    unitController.shooting.StopFire();
                    unitController.movement.MoveRelative(new Vector3(0, 0, 1));
                }
            }
            else
            {
                //print("Covered by an object");
            }
        }
        else
        {
            print("Error. Raycast hit nothing");
        }
    }

    private void FixedUpdate()
    {
        Debug.DrawRay(rigidbody.position, transform.up, Color.green);
        Debug.DrawRay(rigidbody.position, player.transform.position - rigidbody.position, Color.red);
    }
}
