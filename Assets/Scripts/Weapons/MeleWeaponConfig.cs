using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleWeaponConfig", menuName = "ScriptableObjects/WeaponConfig/Mele")]
public class MeleWeaponConfig : WeaponConfig
{
    public int damage;
    public float range;
}
