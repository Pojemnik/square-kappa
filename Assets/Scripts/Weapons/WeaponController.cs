using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PickableItem))]
abstract public class WeaponController : MonoBehaviour
{
    abstract public WeaponConfig config { get; }
}
