using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitShooting : MonoBehaviour
{
    [Header("Refernces")]
    public Unit owner;

    public void Fire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            StartFire();
        }
        else if (context.canceled)
        {
            StopFire();
        }
    }

    public void StartFire()
    {
        if (owner.CurrentWeapon == null || owner.CurrentWeaponController == null)
        {
            return;
        }
        owner.CurrentWeaponController.startShoot();
        owner.AnimationController.SetState("Fire");
    }

    public void StopFire()
    {
        if (owner.CurrentWeapon == null || owner.CurrentWeaponController == null)
        {
            return;
        }
        owner.CurrentWeaponController.stopShoot();
        owner.AnimationController.SetState("StopFire");
    }
}
