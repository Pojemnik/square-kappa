using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        rigidbody = GetComponent<Rigidbody>();
        layerMask = (1 << 7) | (1 << 8) | (1 << 9);
        layerMask = ~layerMask;
    }

    void Update()
    {
        Vector3 positionDelta = player.transform.position - rigidbody.position;
        unitController.LookAt(positionDelta);
        Debug.DrawRay(transform.position, positionDelta);
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
                    unitController.StartFire();
                    //unitController.MoveTowards(Vector3.zero);
                    unitController.MoveRelative(Vector3.zero);
                }
                else
                {
                    unitController.StopFire();
                    //unitController.MoveTowards(positionDelta);
                    unitController.MoveRelative(new Vector3(0, 0, 1));
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
}
