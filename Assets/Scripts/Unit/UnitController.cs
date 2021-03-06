using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitController : Unit
{
    [Header("Events")]
    //TODO fix inventory
    public UnityEngine.Events.UnityEvent<int, string> inventoryChange;
    private Inventory inventory;

    [Header("Components")]
    public UnitMovement movement;
    public UnitShooting shooting;
    public UnitDash dashing;
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
            currentWeaponController = currentWeapon.GetComponent<RangedWeaponController>();
        }
    }

    public void Start()
    {
        inventory = new Inventory(1, 2);
    }

    public void DropItem()
    {
        if (currentWeapon != null)
        {
            itemChanger.DropWeapon();
        }
    }

    public void PickItem()
    {
        itemChanger.PickOrSwapWeapon();
    }

    public void StartDash()
    {
        dashing.EnableDashMode();
    }

    public void CancelDash()
    {
        dashing.DisableDashMode();
    }

    public void PickWeaponFromInventory(int slot)
    {
        GameObject weapon = inventory.GetWeapon(slot);
        if (currentWeapon)
        {
            //some weapon currently in hands
            int insertedSlot = inventory.AddWeapon(currentWeapon);
            if (insertedSlot != -1)
            {
                //there is place for it in inventory
                //print(String.Format("Weapon {0} placed in inventory", currentWeapon.name));
                inventoryChange.Invoke(insertedSlot, currentWeapon.name);
                currentWeapon.SetActive(false);
                currentWeapon = null;
                currentWeaponController = null;
            }
            else
            {
                //there is no place
                //print(String.Format("No space in inventory. Dropping weapon {0}", currentWeapon.name));
                //DropWeapon();
            }
        }
        if (weapon)
        {
            //take weapon from selected slot
            inventoryChange.Invoke(slot, "");
            //GrabWeapon(weapon);
            //print(String.Format("Taken weapon from inventory: {0}", weapon.name));
        }
    }
}
