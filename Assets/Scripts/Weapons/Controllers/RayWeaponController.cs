using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayWeaponController : RangedWeaponController
{
    public override float Spread => 0;

    [SerializeField]
    private RayWeaponConfig rayConfig;

    private RayController projectile;

    private void Awake()
    {
        projectile = Instantiate(rayConfig.projectilePrefab, transform).GetComponent<RayController>();
        projectile.gameObject.SetActive(false);
    }

    public override void StartAttack()
    {
        base.StartAttack();
        projectile.gameObject.SetActive(true);
    }

    public override void StopAttack()
    {
        base.StopAttack();
        projectile.gameObject.SetActive(false);
    }

    private void Shoot()
    {
        Vector3 offset = rayConfig.projectileOffset.x * transform.right + rayConfig.projectileOffset.y * transform.up + rayConfig.projectileOffset.z * transform.forward;
        projectile.DisplayRay(transform.position + offset, transform.forward);
    }

    private void FixedUpdate()
    {
        if(triggerHold)
        {
            Shoot();
        }
    }
}
