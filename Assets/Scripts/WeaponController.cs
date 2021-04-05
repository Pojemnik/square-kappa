using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject flamePrefab;
    public GameObject projectilePrefab;
    public float shootsPerSecond;
    public bool continousShooting;
    public float projectileSpeed;
    public Vector3 projectileOffset;
    public Vector3 flameOffset;
    public Vector3 projectileAngularOffset;
    public float spreadRadius;
    public Vector3 flameRotation;

    private bool triggerHold = false;
    private new Transform transform;
    private float shootCooldown;

    private void Awake()
    {
        flamePrefab.SetActive(false);
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
        Vector3 spread = Random.insideUnitSphere * spreadRadius;
        Vector3 relativeOffset = projectileOffset.x * transform.forward +  projectileOffset.y * transform.right + projectileOffset.z * transform.up;
        Quaternion relativeRotation = Quaternion.Euler(transform.TransformDirection(spread + projectileAngularOffset));
        GameObject projectile = Instantiate(projectilePrefab, transform.position + relativeOffset, transform.rotation * relativeRotation);
        projectile.SetActive(true);
        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();
        projectileRB.AddRelativeForce(projectileSpeed, 0, 0, ForceMode.VelocityChange);
        Destroy(projectile, 3);
        GameObject fireEffect = Instantiate(flamePrefab, transform.position, transform.rotation);
        fireEffect.transform.parent = transform;
        fireEffect.transform.Translate(flameOffset, Space.Self);
        fireEffect.transform.Rotate(flameRotation);
        fireEffect.SetActive(true);
        Destroy(fireEffect, (float)0.1);
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
