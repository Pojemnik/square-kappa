using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RangedWeaponController : WeaponController
{
    public override WeaponConfig Config { get => rangedConfig; }
    public override UnityEvent AttackEvent { get => shootEvent; }
    public override Quaternion AttackDirection { get => projectileDirection; set { projectileDirection = value; } }

    public override float Spread
    {
        get
        {
            return spreadRadius;
        }
    }

    [SerializeField]
    private DisplayController totalAmmoDisplay;
    [SerializeField]
    private DisplayController currentAmmoDisplay;

    [SerializeField]
    private RangedWeaponConfig rangedConfig;
    private bool triggerHold = false;
    private float shootCooldown;
    private float spreadRadius;
    private float spreadReductionCooldown;
    private UnityEvent shootEvent;
    private Quaternion projectileDirection;
    private int ammo;

    private void Awake()
    {
        spreadRadius = rangedConfig.baseSpread;
        shootEvent = new UnityEvent();
    }

    public override void StartAttack()
    {
        StartShoot();
    }

    public override void StopAttack()
    {
        StopShoot();
    }

    private void StartShoot()
    {
        triggerHold = true;
    }

    private void StopShoot()
    {
        triggerHold = false;
        spreadReductionCooldown = rangedConfig.spreadReductionDelay;
    }

    private void Shoot()
    {
        if (ammo <= 0)
        {
            return;
        }
        ammo--;
        currentAmmoDisplay.SetValue(ammo);
        Vector3 projectleSpread = Random.insideUnitSphere * spreadRadius;
        Vector3 relativeOffset = rangedConfig.projectileOffset.x * transform.right + rangedConfig.projectileOffset.y * transform.up + rangedConfig.projectileOffset.z * transform.forward;
        Quaternion relativeRotation = Quaternion.Euler(transform.TransformDirection(projectleSpread + rangedConfig.projectileAngularOffset));
        GameObject projectile = Instantiate(rangedConfig.projectilePrefab, transform.position + relativeOffset, projectileDirection * relativeRotation);
        if (gameObject.layer == 6)
        {
            projectile.layer = 8;
        }
        else if (gameObject.layer == 7)
        {
            projectile.layer = 9;
        }
        ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
        projectileController.speed = rangedConfig.projectileSpeed;
        projectileController.direction = projectile.transform.forward;
        projectile.transform.localScale = Vector3.one * rangedConfig.projectileScale;
        projectile.SetActive(true);
        Destroy(projectile, rangedConfig.projectileLifetime);
        if (rangedConfig.flamePrefab)
        {
            GameObject fireEffect = Instantiate(rangedConfig.flamePrefab, transform.position, transform.rotation);
            fireEffect.transform.parent = transform;
            fireEffect.transform.Translate(rangedConfig.flameOffset, Space.Self);
            fireEffect.transform.Rotate(rangedConfig.flameRotation);
            fireEffect.transform.localScale = Vector3.one * rangedConfig.flameScale;
            fireEffect.SetActive(true);
            Destroy(fireEffect, rangedConfig.fireLifetime);
        }
        if (spreadRadius < rangedConfig.maxSpread)
        {
            spreadRadius += rangedConfig.spreadIncrease;
        }
        else
        {
            spreadRadius = rangedConfig.maxSpread;
        }
        AttackEvent.Invoke();
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
            shootCooldown = 1F / rangedConfig.shootsPerSecond;
            if (!rangedConfig.continousShooting)
            {
                triggerHold = false;
            }
        }
        if (!triggerHold && spreadRadius > rangedConfig.baseSpread)
        {
            spreadReductionCooldown += Time.fixedDeltaTime;
            spreadRadius -= GetCooldownReductionValue(spreadReductionCooldown);
            if (spreadRadius < rangedConfig.baseSpread)
            {
                spreadRadius = rangedConfig.baseSpread;
            }
        }
    }

    private float GetCooldownReductionValue(float time)
    {
        return time * rangedConfig.spreadReductionParameter;
    }

    public override bool AttackAvailable()
    {
        return ammo > 0;
    }

    public void Start()
    {
        ammo = rangedConfig.maxAmmo;
    }

    public override int Reload(int amount)
    {
        if(amount == -1)
        {
            int ammoLeft = ammo;
            ammo = 0;
            currentAmmoDisplay.SetValue(ammo);
            return ammoLeft;
        }
        int total = amount + ammo;
        if (total <= Config.maxAmmo)
        {
            ammo = total;
            total = 0;
        }
        else
        {
            ammo = Config.maxAmmo;
            total -= Config.maxAmmo;
        }
        currentAmmoDisplay.SetValue(ammo);
        return total;
    }

    public override void SetTotalAmmo(int amount)
    {
        totalAmmoDisplay.SetValue(amount);
    }
}
