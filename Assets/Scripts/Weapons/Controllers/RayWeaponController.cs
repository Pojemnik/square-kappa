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
    private CoroutineWrapper shootCoroutine;
    private int layerMask;
    private RaycastHit raycastHit;

    public override Quaternion AttackDirection
    {
        get => projectileDirection;
        set
        {
            projectileDirection = value;
            if(triggerHold)
            {
                Physics.Raycast(projectile.StartPoint, projectileDirection * Vector3.forward, out raycastHit, 1000, layerMask);
            }
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
        shootCoroutine = new CoroutineWrapper(() => Shoot());
    }

    public override void StartAttack()
    {
        base.StartAttack();
        projectile.SetLocalRayDirection(rayConfig.projectileOffset, projectileDirection * Vector3.forward);
        projectile.gameObject.SetActive(true);
        shootCoroutine.StopIfRunning(this);
        SetProjectileLayer(projectile.gameObject);
        CalculateLayerMask();
        Physics.Raycast(projectile.StartPoint, projectileDirection * Vector3.forward, out raycastHit, 1000, layerMask);
        UpdateRayDirection();
        shootCoroutine.Run(this);
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
            ignored = new string[] { "Enemy", "EnemyEnvironmentalCollision", "EnemyProjectile", "Objectives", "Pickups", "PlayerEnvironmentalCollision", };
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
        if (this == null)
        {
            return;
        }
        base.StopAttack();
        projectile?.gameObject?.SetActive(false);
        shootCoroutine.StopIfRunning(this);
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
        if (raycastHit.transform == null)
        {
            projectile.SetLocalRayDirection(projectileDirection * Vector3.forward);
        }
        else
        {
            projectile.SetRayEnd(projectile.transform.InverseTransformPoint(raycastHit.point));
        }
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
                if (raycastHit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
                {
                    Destroy(Instantiate(rayConfig.hitDecal, raycastHit.point + raycastHit.normal * -0.1f, Quaternion.LookRotation(raycastHit.normal), raycastHit.transform), 60);
                }
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
        if (hitEffect == null)
        {
            return;
        }
        if (Physics.Raycast(projectile.StartPoint, projectileDirection * Vector3.forward, out raycastHit, 1000, layerMask))
        {
            hitEffect.transform.position = raycastHit.point;
            hitEffect.transform.rotation = Quaternion.LookRotation(raycastHit.normal);
            hitEffect.SetActive(true);
        }
        else
        {
            hitEffect.SetActive(false);
        }
    }
}
