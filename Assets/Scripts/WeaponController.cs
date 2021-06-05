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
    [HideInInspector]
    public float spread
    {
        get
        {
            return spreadRadius;
        }
    }
    public float baseSpread;
    public float spreadDecrease;
    public float spreadIncrease;
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

    public Quaternion projectileDirection;

    private bool triggerHold = false;
    private new Transform transform;
    private float shootCooldown;
    private float spreadRadius;

    private void Awake()
    {
        flamePrefab.SetActive(false);
        transform = GetComponent<Transform>();
        spreadRadius = baseSpread;
    }

    public void startShoot()
    {
        triggerHold = true;
    }

    public void stopShoot()
    {
        triggerHold = false;
        StartCoroutine(ReduceSpread(shootCooldown));
    }

    private void Shoot()
    {
        Vector3 projectleSpread = Random.insideUnitSphere * spreadRadius;
        Vector3 relativeOffset = projectileOffset.x * transform.right + projectileOffset.y * transform.up + projectileOffset.z * transform.forward;
        Quaternion relativeRotation = Quaternion.Euler(transform.TransformDirection(projectleSpread + projectileAngularOffset));
        GameObject projectile = Instantiate(projectilePrefab, transform.position + relativeOffset, projectileDirection * relativeRotation);
        if (gameObject.layer == 6)
        {
            projectile.layer = 8;
        }
        else if (gameObject.layer == 7)
        {
            projectile.layer = 9;
        }
        ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
        projectileController.speed = projectileSpeed;
        projectileController.direction = projectile.transform.forward;
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
        spreadRadius += spreadIncrease;
    }

    public IEnumerator SetLayerAfterDelay(float time, int layer)
    {
        yield return new WaitForSeconds(time);
        gameObject.layer = layer;
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

    public IEnumerator ReduceSpread(float time)
    {
        yield return new WaitForSeconds(time);
        if (!triggerHold)
        {
            spreadRadius -= spreadDecrease;
            if (spreadRadius <= baseSpread)
            {
                spreadRadius = baseSpread;
            }
            else
            {
                StartCoroutine(ReduceSpread(time));
            }
        }
    }
}
