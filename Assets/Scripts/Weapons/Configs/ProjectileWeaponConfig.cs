using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileWeaponConfig", menuName = "ScriptableObjects/WeaponConfig/Projectile")]
public class ProjectileWeaponConfig : WeaponConfig
{
    [Header("Weapon parameters")]
    public float shootsPerSecond;
    [Tooltip("True - player can hold trigger for automatic shooting")]
    public bool continousShooting;

    [Tooltip("Spread of first shot")]
    public float baseSpread;
    [Tooltip("Upper bound of weapon spread")]
    public float maxSpread;
    [Tooltip("Spread is being increased with each shot by that value")]
    public float spreadIncrease;
    [Tooltip("When weapon is not shooting and <spreadReductionDelay> passes, spread is being reduced every frame by: <time since last shot>*SpreadReductionParameter")]
    public float spreadReductionParameter;
    [Tooltip("When weapon is not shooting and <spreadReductionDelay> passes, spread is being reduced every frame by: <time since last shot>*SpreadReductionParameter")]
    public float spreadReductionDelay;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float projectileSpeed;
    public Vector3 projectileOffset;
    public Vector3 projectileAngularOffset;
    public float projectileScale;
    public float projectileLifetime;

    [Header("Flame")]
    public GameObject flamePrefab;
    public Vector3 flameOffset;
    public Vector3 flameRotation;
    public float flameScale;
    public float fireLifetime;
}
