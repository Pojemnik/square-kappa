using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RayWeaponConfig", menuName = "ScriptableObjects/WeaponConfig/Ray")]
public class RayWeaponConfig : ScriptableObject
{
    public float tickDuration;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Vector3 projectileOffset;

    [Header("Flame")]
    public GameObject flamePrefab;
    public Vector3 flameOffset;
    public Vector3 flameRotation;
    public float flameScale;

    [Header("Hit")]
    public GameObject hitEffectPrefab;

    [Header("Decal")]
    public GameObject hitDecal;
}
