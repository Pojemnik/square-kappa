using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayWeaponController : RangedWeaponController
{
    public override float Spread => 0;

    [SerializeField]
    private RayWeaponConfig rayConfig;

    private RayController projectile;
    private Coroutine shootCoroutine;

    public override Quaternion AttackDirection
    {
        get => projectileDirection;
        set
        {
            projectileDirection = value;
            UpdateRayDirection();
        }
    }

    private void Awake()
    {
        projectile = Instantiate(rayConfig.projectilePrefab, transform).GetComponent<RayController>();
        projectile.gameObject.SetActive(false);
    }

    public override void StartAttack()
    {
        base.StartAttack();
        projectile.SetLocalRayDirection(rayConfig.projectileOffset, projectileDirection * Vector3.forward);
        projectile.gameObject.SetActive(true);
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
        }
        shootCoroutine = StartCoroutine(Shoot());
    }

    public override void StopAttack()
    {
        base.StopAttack();
        projectile.gameObject.SetActive(false);
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
        }
    }

    private void UpdateRayDirection()
    {
        projectile.SetLocalRayDirection(projectileDirection * Vector3.forward);
    }

    private IEnumerator Shoot()
    {
        while (triggerHold)
        {
            yield return new WaitForSeconds(rayConfig.tickDuration);
            //Physics.Raycast(projectile.StartPoint, projectile.Direction);
            Debug.DrawLine(projectile.StartPoint, projectile.StartPoint + projectileDirection * Vector3.forward * 1000, Color.magenta);
        }
    }
}
