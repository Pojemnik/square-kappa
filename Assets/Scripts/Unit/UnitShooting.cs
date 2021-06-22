using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitShooting : MonoBehaviour
{
    [Header("Refernces")]
    public Unit owner;

    private new Rigidbody rigidbody;
    private WeaponController weaponController = null;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (owner.CurrentWeapon == null || owner.CurrentWeaponController == null)
        {
            return;
        }
        if(owner.CurrentWeaponController != weaponController)
        {
            if (weaponController != null)
            {
                weaponController.ShootEvent.RemoveListener(OnWeaponShoot);
            }
            weaponController = owner.CurrentWeaponController;
            weaponController.ShootEvent.AddListener(OnWeaponShoot);
        }
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
        owner.CurrentWeaponController.startShoot();
        owner.AnimationController.SetState("Fire");
    }

    public void StopFire()
    {
        owner.CurrentWeaponController.stopShoot();
        owner.AnimationController.SetState("StopFire");
    }

    private void OnWeaponShoot()
    {
        rigidbody.AddForce(-transform.up * weaponController.config.backwardsForce);
    }
}
