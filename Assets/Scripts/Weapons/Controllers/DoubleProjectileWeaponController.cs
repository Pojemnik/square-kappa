using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleProjectileWeaponController : ProjectileWeaponController
{
    [SerializeField]
    protected new DoubleProjectileWeaponConfig projectileWeaponConfig;

    protected override void Shoot()
    {
        Vector3 relativeOffset = projectileWeaponConfig.projectileOffset.x * transform.right + projectileWeaponConfig.projectileOffset.y * transform.up + projectileWeaponConfig.projectileOffset.z * transform.forward;
        Quaternion relativeRotation = Quaternion.Euler(transform.TransformDirection(projectileWeaponConfig.projectileAngularOffset));
        Vector3 centerOffset = projectileWeaponConfig.offsetFromTheCenter.x * transform.right + projectileWeaponConfig.offsetFromTheCenter.y * transform.up + projectileWeaponConfig.offsetFromTheCenter.z * transform.forward;
        CreateProjectile(relativeOffset + centerOffset, relativeRotation);
        CreateProjectile(relativeOffset - centerOffset, relativeRotation);
        if (projectileWeaponConfig.flamePrefab != null)
        {
            CreateFlame();
        }
        InvokeAttackEvent();
    }
}
