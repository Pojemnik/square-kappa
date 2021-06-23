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
    private WeaponController defaultWeaponController;

    private new Rigidbody rigidbody;
    private WeaponController weaponController = null;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        if (useDefaultWeapon)
        {
            weaponController = defaultWeaponController;
        }
    }

    private void ChangeWeaponController(WeaponController newController)
    {
        if (weaponController != null)
        {
            weaponController.AttackEvent.RemoveListener(OnWeaponShoot);
        }
        weaponController = newController;
        weaponController.AttackEvent.AddListener(OnWeaponShoot);
    }

    private bool UpdateWeaponController()
    {
        if (owner.CurrentWeaponController == null)
        {
            if (useDefaultWeapon)
            {
                ChangeWeaponController(defaultWeaponController);
            }
            else
            {
                return false;
            }
        }
        bool defaultWeaponInUse = useDefaultWeapon && (owner.CurrentWeaponController == null) && weaponController == defaultWeaponController;
        if (owner.CurrentWeaponController != weaponController && !defaultWeaponInUse)
        {
            ChangeWeaponController(owner.CurrentWeaponController);
        }
        return true;
    }

    public void StartFire()
    {
        if (UpdateWeaponController())
        {
            weaponController.StartAttack();
            owner.AnimationController.SetState("Fire");
        }
    }

    public void StopFire()
    {
        if (UpdateWeaponController())
        {
            weaponController.StopAttack();
            owner.AnimationController.SetState("StopFire");
        }
    }

    private void OnWeaponShoot()
    {
        rigidbody.AddForce(-transform.up * weaponController.Config.backwardsForce);
    }
}
