using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class RangedWeaponController : WeaponController
{
    [SerializeField]
    protected WeaponConfig config;
    [SerializeField]
    protected DisplayController totalAmmoDisplay;
    [SerializeField]
    protected DisplayController currentAmmoDisplay;

    protected UnityEvent shootEvent;
    protected Quaternion projectileDirection;
    protected UnityEvent<(int, int)> ammoChangeEvent;
    protected int ammo;
    protected int totalAmmo;
    protected bool triggerHold = false;

    public override WeaponConfig Config { get => config; }
    public override UnityEvent AttackEvent { get => shootEvent; }
    public override Quaternion AttackDirection { get => projectileDirection; set { projectileDirection = value; } }
    public abstract override float Spread { get; }

    public override MagazineStateType MagazineState
    {
        get
        {
            if (ammo == Config.maxAmmo)
            {
                return MagazineStateType.Full;
            }
            if (ammo <= 0)
            {
                return MagazineStateType.Empty;
            }
            return MagazineStateType.NotEmptyNotFull;
        }
    }
    public override UnityEvent<(int, int)> AmmoChangeEvent { get => ammoChangeEvent; }

    protected virtual void Awake()
    {
        shootEvent = new UnityEvent();
        ammoChangeEvent = new UnityEvent<(int, int)>();
    }
    public override void StartAttack()
    {
        StartShoot();
    }

    public override void StopAttack()
    {
        StopShoot();
    }

    protected virtual void StartShoot()
    {
        triggerHold = true;
    }

    protected virtual void StopShoot()
    {
        triggerHold = false;
    }

    protected void Start()
    {
        ammo = Config.maxAmmo;
    }

    public override int Reload(int amount)
    {
        if (amount == -1)
        {
            int ammoLeft = ammo;
            ammo = 0;
            currentAmmoDisplay.SetValue(ammo);
            ammoChangeEvent.Invoke((ammo, totalAmmo));
            return ammoLeft;
        }
        int total = amount + ammo;
        if (total <= Config.maxAmmo)
        {
            ammo = total;
            total = 0;
        }
        else
        {
            ammo = Config.maxAmmo;
            total -= Config.maxAmmo;
        }
        currentAmmoDisplay.SetValue(ammo);
        ammoChangeEvent.Invoke((ammo, totalAmmo));
        return total;
    }

    public override void SetTotalAmmo(int amount)
    {
        totalAmmoDisplay.SetValue(amount);
        totalAmmo = amount;
        ammoChangeEvent.Invoke((ammo, totalAmmo));
    }
}
