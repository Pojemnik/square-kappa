using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitShooting : MonoBehaviour
{
    [HideInInspector]
    public bool IgnoreRecoil;

    [Header("Refernces")]
    [SerializeField]
    private Unit owner;

    [Header("Properties")]
    [SerializeField]
    private bool infiniteAmmo;
    [SerializeField]
    private bool autoReload;

    [Header("Start ammo")]
    [SerializeField]
    [Min(0)]
    private int startChemirailAmmo;

    private new Rigidbody rigidbody;
    private WeaponController weaponController = null;
    private Dictionary<WeaponConfig.WeaponType, int> allAmmo;
    private bool reloading;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        allAmmo = new Dictionary<WeaponConfig.WeaponType, int>();
        foreach (WeaponConfig.WeaponType type in System.Enum.GetValues(typeof(WeaponConfig.WeaponType)))
        {
            allAmmo.Add(type, 0);
        }
        allAmmo[WeaponConfig.WeaponType.Rifle] = startChemirailAmmo;
        reloading = false;
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
        weaponController.SetTotalAmmo(allAmmo[weaponController.Config.type]);
    }

    public void StartFire()
    {
        if (weaponController == null)
        {
            return;
        }
        if (reloading)
        {
            return;
        }
        if (AmmoAvailable())
        {
            weaponController.StartAttack();
            owner.AnimationController.SetStaticState("Fire");
        }
        else
        {
            if (autoReload)
            {
                Reload();
            }
        }
    }

    private bool AmmoAvailable()
    {
        return weaponController.MagazineState != WeaponController.MagazineStateType.Empty;
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
        if (reloading)
        {
            return;
        }
        bool noAmmoInInventory = !infiniteAmmo && allAmmo[weaponController.Config.type] == 0;
        bool magazineFull = weaponController.MagazineState == WeaponController.MagazineStateType.Full;
        if (noAmmoInInventory || magazineFull)
        {
            //No ammo to add
            return;
        }
        reloading = true;
        owner.AnimationController.SetState("Reload");
    }

    public void PickUpAmmo(WeaponConfig.WeaponType type, int amount)
    {
        allAmmo[type] += amount;
        if (weaponController != null && weaponController.Config.type == type)
        {
            weaponController.SetTotalAmmo(allAmmo[type]);
        }
    }

    public void OnReloadEnd()
    {
        if (infiniteAmmo || allAmmo[weaponController.Config.type] >= weaponController.Config.maxAmmo)
        {

            int ammoLeft = weaponController.Reload(weaponController.Config.maxAmmo);
            allAmmo[weaponController.Config.type] -= weaponController.Config.maxAmmo;
            allAmmo[weaponController.Config.type] += ammoLeft;
        }
        else
        {
            int ammoLeft = weaponController.Reload(allAmmo[weaponController.Config.type]);
            allAmmo[weaponController.Config.type] = ammoLeft;
        }
        if (!infiniteAmmo)
        {
            weaponController.SetTotalAmmo(allAmmo[weaponController.Config.type]);
        }
        reloading = false;
    }

    private void OnWeaponShoot()
    {
        if (!IgnoreRecoil)
        {
            rigidbody.AddForce(-transform.up * weaponController.Config.backwardsForce);
        }
        if (!AmmoAvailable())
        {
            StopFire();
        }
    }
}
