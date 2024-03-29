using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "ScriptableObjects/WeaponConfig/Base")]
public class WeaponConfig : ScriptableObject
{
    [System.Serializable]
    public enum WeaponSlotType
    {
        Small,
        Big,
        Mele
    }

    [System.Serializable]
    public enum WeaponType
    {
        LaserPistol,
        Rifle,
        LaserRifle,
        Fists
    }
    [Tooltip("Type of slot taken by weapon in inventory")]
    public WeaponSlotType slotType;
    [Tooltip("Weapon type used for selecting animation")]
    public WeaponType type;
    [Tooltip("Backwards force applied to shooter while shooting")]
    public float backwardsForce;
    public int maxAmmo;
    public float length;
    public float wallDistanceWhenPulled;
    public int damage;
}
