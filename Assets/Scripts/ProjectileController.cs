using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public GameObject hitEffectPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(Instantiate(hitEffectPrefab, collision.GetContact(0).point, Quaternion.Euler(collision.GetContact(0).normal)), 1);
        Destroy(gameObject);
    }
}
