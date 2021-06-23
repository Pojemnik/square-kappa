using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public virtual GameObject CurrentWeapon { get; set; }
    public virtual WeaponController CurrentWeaponController { get; }
    public virtual UnitAnimationController AnimationController { get; }
    public virtual Quaternion TowardsTarget { get; set; }
}
