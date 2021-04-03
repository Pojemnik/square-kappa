using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float startSpeed;
    public GameObject hitEffectPrefab;

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

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.transform.gameObject.name);
        Destroy(Instantiate(hitEffectPrefab, collision.GetContact(0).point, Quaternion.Euler(collision.GetContact(0).normal)), 1);
        Destroy(gameObject);
    }
}
