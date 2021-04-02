using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float startSpeed;

    private Rigidbody rb;
    private float speed;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = startSpeed;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + rb.transform.right * speed * Time.fixedDeltaTime);
    }
}
