using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class RegularProjectileController : ProjectileController
{
    private new Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddRelativeForce(0, 0, speed, ForceMode.VelocityChange);
    }

    private GameObject GetParentWithHealth(Transform transform)
    {
        while (transform.parent != null && !transform.gameObject.CompareTag("Player") && !transform.gameObject.CompareTag("Enemy"))
        {
            transform = transform.parent;
        }
        return transform.gameObject;
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
        GameObject topParent = GetParentWithHealth(other.transform);
        Health othersHealth = topParent.gameObject.GetComponent<Health>();
        if (othersHealth != null)
        {
            othersHealth.Damaged(new DamageInfo(damage, direction));
        }
        Destroy(Instantiate(hitEffectPrefab, collision.GetContact(0).point, Quaternion.Euler(collision.GetContact(0).normal)), 5);
        Destroy(gameObject);
    }
}
