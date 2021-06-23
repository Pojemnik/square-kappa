using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitShooting : MonoBehaviour
{
    [Header("Refernces")]
    public Unit owner;

    [Header("Default weapon")]
    [SerializeField]
    private bool useDefaultWeapon;
    [SerializeField]
    private RangedWeaponController defaultWeaponController;

    private new Rigidbody rigidbody;
    private RangedWeaponController weaponController = null;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        if (useDefaultWeapon)
        {
            weaponController = defaultWeaponController;
        }
    }

    private void ChangeWeaponController(RangedWeaponController newController)
    {
        if (weaponController != null)
        {
            weaponController.ShootEvent.RemoveListener(OnWeaponShoot);
        }
        weaponController = newController;
        weaponController.ShootEvent.AddListener(OnWeaponShoot);
    }

    public void StartFire()
    {
        if (owner.CurrentWeaponController == null)
        {
            if (useDefaultWeapon)
            {
                ChangeWeaponController(defaultWeaponController);
            }
            else
            {
                return;
            }
        }
        bool defaultWeaponInUse = useDefaultWeapon && (owner.CurrentWeaponController == null) && weaponController == defaultWeaponController;
        if (owner.CurrentWeaponController != weaponController && !defaultWeaponInUse)
        {
            ChangeWeaponController(owner.CurrentWeaponController);
        }
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
