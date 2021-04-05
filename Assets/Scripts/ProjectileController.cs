using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public GameObject hitEffectPrefab;

    private new Rigidbody rigidbody;
    private new Collider collider;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(Instantiate(hitEffectPrefab, collision.GetContact(0).point, Quaternion.Euler(collision.GetContact(0).normal)), 1);
        Destroy(rigidbody);
        Destroy(collider);
        Destroy(gameObject, 1);
    }
}
