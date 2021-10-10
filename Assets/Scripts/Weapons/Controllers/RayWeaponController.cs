using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayWeaponController : RangedWeaponController
{
    public override float Spread => 0;

    [SerializeField]
    private RayWeaponConfig rayConfig;

    private GameObject flame;
    private GameObject hitEffect;
    private RayController projectile;
    private Coroutine shootCoroutine;
    private int layerMask;
    private RaycastHit raycastHit;

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
        hitEffect = Instantiate(rayConfig.hitEffectPrefab);
        hitEffect.SetActive(false);
        flame = Instantiate(rayConfig.flamePrefab, transform);
        flame.transform.localPosition = rayConfig.flameOffset;
        flame.transform.localScale = Vector3.one * rayConfig.flameScale;
        flame.transform.localRotation = Quaternion.Euler(rayConfig.flameRotation);
        flame.SetActive(false);
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
        SetProjectileLayer(projectile.gameObject);
        CalculateLayerMask();
        Physics.Raycast(projectile.StartPoint, projectileDirection * Vector3.forward, out raycastHit, 1000, layerMask);
        shootCoroutine = StartCoroutine(Shoot());
        flame.SetActive(true);
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
            int layer = LayerMask.NameToLayer(s);
            if (layer == -1)
            {
                Debug.LogErrorFormat("No layer called {0} found", s);
            }
            else
            {
                layerMask |= (1 << layer);
            }
        }
        layerMask = ~layerMask;
    }

    public override void StopAttack()
    {
        if(this == null)
        {
            return;
        }
        base.StopAttack();
        projectile?.gameObject?.SetActive(false);
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
        }
        if (hitEffect != null)
        {
            hitEffect.SetActive(false);
        }
        if (flame != null)
        {
            flame.SetActive(false);
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
            if (ammo <= 0)
            {
                triggerHold = false;
                StopAttack();
                break;
            }
            ammo--;
            currentAmmoDisplay.SetValue(ammo);
            InvokeAmmoChengeEvent(ammo, totalAmmo);
            if (raycastHit.collider != null)
            {
                Health health = GetParentsHealth(raycastHit.transform);
                if (health == null)
                {
                    yield return new WaitForSeconds(rayConfig.tickDuration);
                    continue;
                }
                DamageInfo info = new DamageInfo(config.damage, projectileDirection * Vector3.forward, raycastHit.point, raycastHit.normal);
                health.Damaged(info);
            }
            yield return new WaitForSeconds(rayConfig.tickDuration);
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

    private void Update()
    {
        if (!triggerHold)
        {
            return;
        }
        Debug.DrawLine(projectile.StartPoint, projectile.StartPoint + projectileDirection * Vector3.forward * 1000, Color.magenta);
        if(hitEffect == null)
        {
            return;
        }
        if (Physics.Raycast(projectile.StartPoint, projectileDirection * Vector3.forward, out raycastHit, 1000, layerMask))
        {
            hitEffect.SetActive(true);
            hitEffect.transform.position = raycastHit.point;
            hitEffect.transform.rotation = Quaternion.LookRotation(raycastHit.normal);
        }
        else
        {
            hitEffect.SetActive(false);
        }
    }
}
