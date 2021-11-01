using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitController : Unit
{
    [Header("Components")]
    public UnitMovement movement;
    public UnitShooting shooting;
    public ItemChanger itemChanger;
    [SerializeField]
    private UnitAnimationController animationController;
    public override UnitAnimationController AnimationController { get => animationController; }

    private WeaponController currentWeaponController;
    public override WeaponController CurrentWeaponController { get => currentWeaponController; }
    private GameObject currentWeapon;
    public override GameObject CurrentWeapon
    {
        get { return currentWeapon; }
        set
        {
            currentWeapon = value;
            if (currentWeapon)
            {
                currentWeaponController = currentWeapon.GetComponent<WeaponController>();
            }
            else
            {
                currentWeaponController = null;
            }
        }
    }
    
    public override Quaternion TowardsTarget
    {
        get { return targetDirection; }
        set
        {
            targetDirection = value;
            if (currentWeaponController != null)
            {
                currentWeaponController.AttackDirection = targetDirection;
            }
        }
    }
    private Quaternion targetDirection;

    private void Awake()
    {
        //get components
        if (currentWeapon != null)
        {
            currentWeaponController = currentWeapon.GetComponent<WeaponController>();
        }
    }
}
