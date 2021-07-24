using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitShooting : MonoBehaviour
{
    [HideInInspector]
    public bool IgnoreRecoil;
    [HideInInspector]
    public bool NeedsReload
    {
        get
        {
            if(weaponController != null)
            {
                if(!weaponController.AttackAvailable())
                {
                    return true;
                }
            }
            return false;
        }
    }

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
            if (weaponController.AttackAvailable())
            {
                weaponController.StartAttack();
                owner.AnimationController.SetStaticState("Fire");
            }
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

    public void Reload()
    {
        weaponController.Reload();
    }    

    private void OnWeaponShoot()
    {
        if (!IgnoreRecoil)
        {
            rigidbody.AddForce(-transform.up * weaponController.Config.backwardsForce);
        }
        if(!weaponController.AttackAvailable())
        {
            StopFire();
        }
    }
}
