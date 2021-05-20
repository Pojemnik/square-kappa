using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularProjectileController : ProjectileController
{
    private new Rigidbody rigidbody;
    private new Collider collider;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        rigidbody.AddRelativeForce(speed, 0, 0, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (int layer in ignoredLayers)
        {
            if (collision.collider.gameObject.layer == layer)
            {
                return;
            }
        }
        Destroy(Instantiate(hitEffectPrefab, collision.GetContact(0).point, Quaternion.Euler(collision.GetContact(0).normal)), 1);
        Destroy(rigidbody);
        Destroy(collider);
        Destroy(gameObject, 1);
    }
}
