using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "ScriptableObjects/WeaponConfig/Base")]
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
        Rifle,
        Fists
    }
    [Tooltip("Size of slot taken by weapon in inventory")]
    public WeaponSize size;
    [Tooltip("Weapon type used for selecting animation")]
    public WeaponType type;
    [Tooltip("Backwards force applied to shooter while shooting")]
    public float backwardsForce;
    public int maxAmmo;
    public float length;
}
