using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleWeaponConfig", menuName = "ScriptableObjects/WeaponConfig/Mele")]
public class MeleWeaponConfig : WeaponConfig
{
    public float range;
    [Tooltip("Time between attack start and trial of dealing damage to the target")]
    public float damageTimeDelta;
}
