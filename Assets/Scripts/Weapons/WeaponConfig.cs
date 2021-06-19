using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "ScriptableObjects/WeaponConfig")]
public class WeaponConfig : ScriptableObject
{
    [System.Serializable]
    public enum WeaponSize
    {
        Small,
        Big
    }

    [System.Serializable]
    public enum WeaponType
    {
        Pistol,
        Rifle
    }

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
    [Tooltip("Size of slot taken by weapon in inventory")]
    public WeaponSize size;
    [Tooltip("Weapon type used for selecting animation")]
    public WeaponType type;

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
