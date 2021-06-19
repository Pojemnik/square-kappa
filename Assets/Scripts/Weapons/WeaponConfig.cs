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
    public bool continousShooting;
    
    public float baseSpread;
    public float maxSpread;
    public float spreadDecrease;
    public float spreadIncrease;
    public WeaponSize size;
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
