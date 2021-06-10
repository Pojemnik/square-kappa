using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Unit
{
    [Header("Events")]
    public UnityEngine.Events.UnityEvent<Vector3> hitEvent;

    //TODO fix inventory
    public UnityEngine.Events.UnityEvent<int, string> inventoryChange;
    private Inventory inventory;

    public UnitMovement movement;
    public UnitShooting shooting;
    public UnitDash dashing;
    public ItemChanger itemChanger;

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
    [SerializeField]
    private Animator playerAnimator;
    public override Animator UnitAnimator { get { return playerAnimator; } set { playerAnimator = value; } }
    public override Quaternion TowardsTarget
    {
        get { return targetDirection; }
        set
        {
            targetDirection = value;
            if (currentWeaponController != null)
            {
                currentWeaponController.projectileDirection = targetDirection;
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

    public void Start()
    {
        //init camera
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //init inventory
        inventory = new Inventory(1, 2);

        if (currentWeapon != null)
        {
            if (currentWeaponController.type == WeaponController.WeaponType.Rifle)
            {
                SetAnimatorLayer("Chemirail");
            }
            else if (currentWeaponController.type == WeaponController.WeaponType.Pistol)
            {
                SetAnimatorLayer("Laser Pistol");
            }
        }
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
        if (currentWeapon)
        {
            itemChanger.SwapWeapons();
        }
        else
        {
            itemChanger.PickWeaponUp();
        }
    }

    public void StartDash()
    {
        dashing.EnableDashMode();
    }

    public void CancelDash()
    {
        dashing.DisableDashMode();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.layer == 8 && gameObject.layer == 7) || (collision.gameObject.layer == 9 && gameObject.layer == 6))
        {
            ProjectileController projectile = collision.gameObject.GetComponent<ProjectileController>();
            if (projectile)
            {
                hitEvent.Invoke(projectile.direction);
            }
        }
    }

    public void SetAnimatorLayer(string name)
    {
        int index = playerAnimator.GetLayerIndex(name);
        playerAnimator.SetLayerWeight(index, 1);
        for (int i = 1; i < playerAnimator.layerCount; i++)
        {
            if (i != index)
            {
                playerAnimator.SetLayerWeight(i, 0);
            }
        }
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
