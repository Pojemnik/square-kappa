using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitShooting : MonoBehaviour
{
    [Header("Refernces")]
    public Unit owner;

    private WeaponController currentWeaponController;

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
        if (owner.CurrentWeapon == null || currentWeaponController == null)
        {
            return;
        }
        currentWeaponController.startShoot();
        owner.UnitAnimator.SetTrigger("Fire");
    }

    public void StopFire()
    {
        if (owner.CurrentWeapon == null || currentWeaponController == null)
        {
            return;
        }
        currentWeaponController.stopShoot();
        owner.UnitAnimator.SetTrigger("StopFire");
    }

    public void OnWeaponChange(WeaponController controller)
    {
        currentWeaponController = controller;
    }

    private void Awake()
    {
        if (owner.CurrentWeapon != null)
        {
            currentWeaponController = owner.CurrentWeapon.GetComponent<WeaponController>();
        }
    }
}
