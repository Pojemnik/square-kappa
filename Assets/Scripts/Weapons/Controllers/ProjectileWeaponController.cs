using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileWeaponController : RangedWeaponController
{
    [SerializeField]
    private ProjectileWeaponConfig projectileWeaponConfig;

    private float shootCooldown;
    private float spreadRadius;
    private float spreadReductionCooldown;

    public override float Spread
    {
        get
        {
            return spreadRadius;
        }
    }

    protected void Awake()
    {
        spreadRadius = projectileWeaponConfig.baseSpread;
    }

    public override void StopAttack()
    {
        base.StopAttack();
        spreadReductionCooldown = projectileWeaponConfig.spreadReductionDelay;
    }

    private void Shoot()
    {
        if (ammo <= 0)
        {
            return;
        }
        ammo--;
        currentAmmoDisplay.SetValue(ammo);
        InvokeAmmoChengeEvent(ammo, totalAmmo);
        Vector3 projectleSpread = Random.insideUnitSphere * spreadRadius;
        Vector3 relativeOffset = projectileWeaponConfig.projectileOffset.x * transform.right + projectileWeaponConfig.projectileOffset.y * transform.up + projectileWeaponConfig.projectileOffset.z * transform.forward;
        Quaternion relativeRotation = Quaternion.Euler(transform.TransformDirection(projectleSpread + projectileWeaponConfig.projectileAngularOffset));
        GameObject projectile = Instantiate(projectileWeaponConfig.projectilePrefab, transform.position + relativeOffset, projectileDirection * relativeRotation);
        if (gameObject.layer == 6)
        {
            projectile.layer = 8;
        }
        else if (gameObject.layer == 7)
        {
            projectile.layer = 9;
        }
        ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
        projectileController.speed = projectileWeaponConfig.projectileSpeed;
        projectileController.direction = projectile.transform.forward;
        projectile.transform.localScale = Vector3.one * projectileWeaponConfig.projectileScale;
        projectile.SetActive(true);
        Destroy(projectile, projectileWeaponConfig.projectileLifetime);
        if (projectileWeaponConfig.flamePrefab)
        {
            GameObject fireEffect = Instantiate(projectileWeaponConfig.flamePrefab, transform.position, transform.rotation);
            fireEffect.transform.parent = transform;
            fireEffect.transform.Translate(projectileWeaponConfig.flameOffset, Space.Self);
            fireEffect.transform.Rotate(projectileWeaponConfig.flameRotation);
            fireEffect.transform.localScale = Vector3.one * projectileWeaponConfig.flameScale;
            fireEffect.SetActive(true);
            Destroy(fireEffect, projectileWeaponConfig.fireLifetime);
        }
        if (spreadRadius < projectileWeaponConfig.maxSpread)
        {
            spreadRadius += projectileWeaponConfig.spreadIncrease;
        }
        else
        {
            spreadRadius = projectileWeaponConfig.maxSpread;
        }
        InvokeAttackEvent();
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
            shootCooldown = 1F / projectileWeaponConfig.shootsPerSecond;
            if (!projectileWeaponConfig.continousShooting)
            {
                triggerHold = false;
            }
        }
        if (!triggerHold && spreadRadius > projectileWeaponConfig.baseSpread)
        {
            spreadReductionCooldown += Time.fixedDeltaTime;
            spreadRadius -= GetCooldownReductionValue(spreadReductionCooldown);
            if (spreadRadius < projectileWeaponConfig.baseSpread)
            {
                spreadRadius = projectileWeaponConfig.baseSpread;
            }
        }
    }

    private float GetCooldownReductionValue(float time)
    {
        return time * projectileWeaponConfig.spreadReductionParameter;
    }
}
