using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject fireEffectPrefab;
    public GameObject projectilePrefab;
    public float shootsPerSecond;
    public bool continousShooting;
    public float projectileSpeed;
    public Vector3 projectileOffset;

    private bool triggerHold = false;
    private new Transform transform;
    private float shootCooldown;

    private void Awake()
    {
        fireEffectPrefab.SetActive(false);
        transform = GetComponent<Transform>();
    }

    public void startShoot()
    {
        triggerHold = true;
    }

    public void stopShoot()
    {
        triggerHold = false;
    }

    private void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        projectile.transform.Translate(projectileOffset, Space.Self);
        projectile.SetActive(true);
        ProjectileController controller = projectile.GetComponent<ProjectileController>();
        controller.startSpeed = projectileSpeed;
        GameObject fireEffect = Instantiate(fireEffectPrefab, transform.position, transform.rotation);
        fireEffect.SetActive(true);
        fireEffect.transform.Translate(projectileOffset, Space.Self);
        Destroy(fireEffect, (float)0.05);
    }

    private void FixedUpdate()
    {
        if(shootCooldown > 0)
        {
            shootCooldown -= Time.fixedDeltaTime;
        }
        if(triggerHold && shootCooldown <= 0)
        {
            Shoot();
            shootCooldown = (float)1 / shootsPerSecond;
            if(!continousShooting)
            {
                triggerHold = false;
            }
        }
    }
}
