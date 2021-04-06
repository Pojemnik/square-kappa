using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject player;

    private PlayerController unitController;
    private new Rigidbody rigidbody;

    void Start()
    {
        unitController = GetComponent<PlayerController>();
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 positionDelta = rigidbody.position - player.transform.position;
        unitController.LookAt(positionDelta);
    }
}
