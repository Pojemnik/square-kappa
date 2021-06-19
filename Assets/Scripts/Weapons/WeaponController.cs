using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PickableItem))]
public class WeaponController : MonoBehaviour
{
    public WeaponConfig config;

    [HideInInspector]
    public float spread
    {
        get
        {
            return spreadRadius;
        }
    }

    [HideInInspector]
    public Quaternion projectileDirection;

    private bool triggerHold = false;
    private new Transform transform;
    private float shootCooldown;
    private float spreadRadius;
    private float spreadReductionCooldown;

    private void Awake()
    {
        config.flamePrefab.SetActive(false);
        transform = GetComponent<Transform>();
        spreadRadius = config.baseSpread;
    }

    public void startShoot()
    {
        triggerHold = true;
    }

    public void stopShoot()
    {
        triggerHold = false;
        spreadReductionCooldown = config.spreadReductionDelay;
    }

    private void Shoot()
    {
        Vector3 projectleSpread = Random.insideUnitSphere * spreadRadius;
        Vector3 relativeOffset = config.projectileOffset.x * transform.right + config.projectileOffset.y * transform.up + config.projectileOffset.z * transform.forward;
        Quaternion relativeRotation = Quaternion.Euler(transform.TransformDirection(projectleSpread + config.projectileAngularOffset));
        GameObject projectile = Instantiate(config.projectilePrefab, transform.position + relativeOffset, projectileDirection * relativeRotation);
        if (gameObject.layer == 6)
        {
            projectile.layer = 8;
        }
        else if (gameObject.layer == 7)
        {
            projectile.layer = 9;
        }
        ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
        projectileController.speed = config.projectileSpeed;
        projectileController.direction = projectile.transform.forward;
        projectile.transform.localScale = Vector3.one * config.projectileScale;
        projectile.SetActive(true);
        Destroy(projectile, config.projectileLifetime);
        if (config.flamePrefab)
        {
            GameObject fireEffect = Instantiate(config.flamePrefab, transform.position, transform.rotation);
            fireEffect.transform.parent = transform;
            fireEffect.transform.Translate(config.flameOffset, Space.Self);
            fireEffect.transform.Rotate(config.flameRotation);
            fireEffect.transform.localScale = Vector3.one * config.flameScale;
            fireEffect.SetActive(true);
            Destroy(fireEffect, config.fireLifetime);
        }
        if (spreadRadius < config.maxSpread)
        {
            spreadRadius += config.spreadIncrease;
        }
        else
        {
            spreadRadius = config.maxSpread;
        }
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
            shootCooldown = 1F / config.shootsPerSecond;
            if (!config.continousShooting)
            {
                triggerHold = false;
            }
        }
        if (!triggerHold && spreadRadius > config.baseSpread)
        {
            spreadReductionCooldown += Time.fixedDeltaTime;
            spreadRadius -= GetCooldownReductionValue(spreadReductionCooldown);
            if(spreadRadius < config.baseSpread)
            {
                spreadRadius = config.baseSpread;
            }
        }
    }

    private float GetCooldownReductionValue(float time)
    {
        return time * config.spreadReductionParameter;
    }    
}
