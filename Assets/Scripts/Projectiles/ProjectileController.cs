using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class ProjectileController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    protected GameObject hitEffectPrefab;

    [Header("Parameters")]
    [HideInInspector]
    public int damage;
    [SerializeField]
    protected float mass;
    [HideInInspector]
    public int[] ignoredLayers;
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public Vector3 direction;

    private new Rigidbody rigidbody;
    private int[] notPushedLayers;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddRelativeForce(0, 0, speed, ForceMode.VelocityChange);
    }

    private void Awake()
    {
        notPushedLayers = new int[4];
        notPushedLayers[0] = LayerMask.NameToLayer("Player");
        notPushedLayers[1] = LayerMask.NameToLayer("Enemy");
        notPushedLayers[2] = LayerMask.NameToLayer("PlayerProjectile");
        notPushedLayers[3] = LayerMask.NameToLayer("EnemyProjectile");
    }

    private bool IsPushable(GameObject target)
    {
        foreach (int i in notPushedLayers)
        {
            if (target.layer == i)
            {
                return false;
            }
        }
        return true;
    }

    private GameObject GetParentWithHealth(Transform transform)
    {
        while (transform.parent != null && transform.gameObject.GetComponent<Health>() == null)
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
        if (IsPushable(other))
        {
            Rigidbody othersRb = other.GetComponent<Rigidbody>();
            if (othersRb == null)
            {
                return;
            }
            float forceValue = mass * collision.relativeVelocity.magnitude / othersRb.mass;
            othersRb.AddForceAtPosition(forceValue * direction.normalized, collision.GetContact(0).point, ForceMode.VelocityChange);
        }
        Health othersHealth = topParent.gameObject.GetComponent<Health>();
        if (othersHealth != null)
        {
            ContactPoint contact = collision.GetContact(0);
            othersHealth.Damaged(new DamageInfo(damage, direction, contact.point, contact.normal));
        }
        Destroy(Instantiate(hitEffectPrefab, collision.GetContact(0).point, Quaternion.identity), 5);
        Destroy(gameObject);
    }
}
