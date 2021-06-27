using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitShooting : MonoBehaviour
{
    [Header("Refernces")]
    [SerializeField]
    private Unit owner;

    private new Rigidbody rigidbody;
    private WeaponController weaponController = null;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void ChangeWeaponController(WeaponController newController)
    {
        if (weaponController != null)
        {
            weaponController.AttackEvent.RemoveListener(OnWeaponShoot);
        }
        weaponController = newController;
        if (weaponController != null)
        {
            weaponController.AttackEvent.AddListener(OnWeaponShoot);
        }
    }

    public void StartFire()
    {
        if (weaponController != null)
        {
            weaponController.StartAttack();
            owner.AnimationController.SetStaticState("Fire");
        }
    }

    public void StopFire()
    {
        if (weaponController != null)
        {
            weaponController.StopAttack();
            owner.AnimationController.ResetStaticState("Fire");
        }
    }

    private void OnWeaponShoot()
    {
        rigidbody.AddForce(-transform.up * weaponController.Config.backwardsForce);
    }
}
