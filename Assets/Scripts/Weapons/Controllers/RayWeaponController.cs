using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayWeaponController : RangedWeaponController
{
    public override float Spread => 0;

    [SerializeField]
    private RayWeaponConfig rayConfig;

    private GameObject flame;
    private RayController projectile;
    private Coroutine shootCoroutine;
    private int layerMask;

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
        SetProjectileLayer(projectile.gameObject);
        CalculateLayerMask();
    }

    private void CalculateLayerMask()
    {
        string[] ignored;
        if (gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            ignored = new string[] { "Player", "PlayerEnvironmentalCollision", "PlayerProjectile", "Objectives", "Pickups" };
        }
        else
        {
            ignored = new string[] { "Enemy", "EnemyEnvironmentalCollision", "EnemyProjectile", "Objectives", "Pickups" };
        }
        layerMask = 0;
        foreach (string s in ignored)
        {
            layerMask |= LayerMask.NameToLayer(s);
        }
        layerMask = ~layerMask;
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
            Debug.DrawLine(projectile.StartPoint, projectile.StartPoint + projectileDirection * Vector3.forward * 1000, Color.magenta);
            if(Physics.Raycast(projectile.StartPoint, projectileDirection * Vector3.forward, out RaycastHit raycastHit, 1000, layerMask))
            {
                Destroy(Instantiate(rayConfig.hitEffectPrefab, raycastHit.point, Quaternion.Euler(raycastHit.normal)), 0.5f);
                Health health = GetParentsHealth(raycastHit.collider.transform);
                if(health == null)
                {
                    continue;
                }
                DamageInfo info = new DamageInfo(config.damage, projectileDirection * Vector3.back, raycastHit.point, raycastHit.normal);
                health.Damaged(info);
            }
        }
    }

    private Health GetParentsHealth(Transform transform)
    {
        Health health = transform.gameObject.GetComponent<Health>();
        while (transform.parent != null && health == null)
        {
            health = transform.gameObject.GetComponent<Health>();
            transform = transform.parent;
        }
        return health;
    }
}
