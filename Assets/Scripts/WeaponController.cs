using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PickableItem))]
public class WeaponController : MonoBehaviour
{
    [System.Serializable]
    public enum WeaponSize
    {
        Small,
        Big
    }

    [System.Serializable]
    public enum WeaponType
    {
        Pistol,
        Rifle
    }

    [Header("Weapon parameters")]
    public float shootsPerSecond;
    public bool continousShooting;
    public float spreadRadius;
    public WeaponSize size;
    public WeaponType type;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float projectileSpeed;
    public Vector3 projectileOffset;
    public Vector3 projectileAngularOffset;
    public float projectileScale;
    public float projectileLifetime;

    [Header("Flame")]
    public GameObject flamePrefab;
    public Vector3 flameOffset;
    public Vector3 flameRotation;
    public float flameScale;
    public float fireLifetime;

    private bool triggerHold = false;
    private new Transform transform;
    private float shootCooldown;

    private void Awake()
    {
        flamePrefab.SetActive(false);
        transform = GetComponent<Transform>();
    }

    public void startShoot()
    {
        triggerHold = true;
    }

    public void stopShoot()
    {
        triggerHold = false;
    }

    private void Shoot()
    {
        Vector3 spread = Random.insideUnitSphere * spreadRadius;
        Vector3 relativeOffset = projectileOffset.x * transform.forward + projectileOffset.y * transform.right + projectileOffset.z * transform.up;
        Quaternion relativeRotation = Quaternion.Euler(transform.TransformDirection(spread + projectileAngularOffset));
        GameObject projectile = Instantiate(projectilePrefab, transform.position + relativeOffset, transform.rotation * relativeRotation);
        if(gameObject.layer == 6)
        {
            projectile.layer = 8;
        }
        else if(gameObject.layer == 7)
        {
            projectile.layer = 9;
        }
        projectile.GetComponent<ProjectileController>().speed = projectileSpeed;
        projectile.transform.localScale = Vector3.one * projectileScale;
        projectile.SetActive(true);
        Destroy(projectile, projectileLifetime);
        if (flamePrefab)
        {
            GameObject fireEffect = Instantiate(flamePrefab, transform.position, transform.rotation);
            fireEffect.transform.parent = transform;
            fireEffect.transform.Translate(flameOffset, Space.Self);
            fireEffect.transform.Rotate(flameRotation);
            fireEffect.transform.localScale = Vector3.one * flameScale;
            fireEffect.SetActive(true);
            Destroy(fireEffect, fireLifetime);
        }
    }

    private void FixedUpdate()
    {
        if (shootCooldown > 0)
        {
            shootCooldown -= Time.fixedDeltaTime;
        }
        if (triggerHold && shootCooldown <= 0)
        {
            Shoot();
            shootCooldown = 1F / shootsPerSecond;
            if (!continousShooting)
            {
                triggerHold = false;
            }
        }
    }
}
