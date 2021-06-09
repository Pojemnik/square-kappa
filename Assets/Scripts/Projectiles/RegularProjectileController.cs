using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class RegularProjectileController : ProjectileController
{
    private new Rigidbody rigidbody;
    private new Collider collider;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        rigidbody.AddRelativeForce(0, 0, speed, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.collider.gameObject;
        foreach (int layer in ignoredLayers)
        {
            if (other.layer == layer)
            {
                return;
            }
        }
        Health othersHealth = other.GetComponent<Health>();
        if(othersHealth)
        {
            othersHealth.Damaged(damage);
        }
        Destroy(Instantiate(hitEffectPrefab, collision.GetContact(0).point, Quaternion.Euler(collision.GetContact(0).normal)), 5);
        Destroy(gameObject);
    }
}
