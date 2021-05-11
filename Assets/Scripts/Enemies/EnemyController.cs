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
        if(positionDelta.magnitude > visionRange)
        {
            print("Too far to see");
            return;
        }
        if (Physics.Raycast(transform.position, positionDelta, visionRange, layerMask))
        {
            if (positionDelta.magnitude < shootingRange)
            {
                unitController.Fire(true);
                unitController.MoveTowards(Vector3.zero);
                print("fire");
            }
            else
            {
                unitController.Fire(false);
                unitController.MoveTowards(positionDelta);
            }
        }
    }
}
