using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public virtual GameObject CurrentWeapon { get; set; }
    public virtual Animator UnitAnimator { get; set; }
    public virtual Quaternion TowardsTarget { get; set; }
}
