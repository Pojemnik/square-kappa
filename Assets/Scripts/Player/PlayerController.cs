using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Unit
{
    public GameObject rightHand;
    public UnityEngine.Events.UnityEvent<int, string> inventoryChange;
    public WeaponController currentWeaponController;
    [HideInInspector]
    public bool shootInCameraDirection = true;
    [SerializeField]
    private Animator playerAnimator;

    [Header("Item pickup")]
    public float weaponPickupRange;
    public float weaponThrowForce;

    [Header("Events")]
    public UnityEngine.Events.UnityEvent<Vector3> hitEvent;
    public UnityEngine.Events.UnityEvent<WeaponController> weaponChangeEvent;

    private GameObject selectedItem;
    private Health health;
    private Inventory inventory;
    
    private PlayerCameraController cameraController;
    [SerializeField]
    private GameObject firstPresonCamera;
    private GameObject currentWeapon;

    public UnitMovement movement;
    public UnitShooting shooting;
    public UnitDash dashing;

    public override GameObject CurrentWeapon { get { return currentWeapon; } set { currentWeapon = value; } }
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
        health = GetComponent<Health>();
        if (currentWeapon != null)
        {
            currentWeaponController = currentWeapon.GetComponent<WeaponController>();
        }
        cameraController = firstPresonCamera.GetComponent<PlayerCameraController>();
    }

    public void Start()
    {
        //init camera
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //init inventory
        inventory = new Inventory(1, 2);
        cameraController.ignoredLayers = new int[2] { 6, 7 };
        cameraController.targettingRange = weaponPickupRange;
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

    public void ActionOne(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        if (currentWeapon != null)
        {
            DropWeapon();
        }
    }

    public void ActionTwo(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        if (currentWeapon)
        {
            SwapWeapons();
        }
        else
        {
            PickWeaponUp();
        }
    }

    public void ActionThree(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            dashing.EnableDashMode();
        }
        if (context.canceled)
        {
            dashing.DisableDashMode();
        }
    }

    private void SwapWeapons()
    {
        if (selectedItem)
        {
            DropWeapon();
            PickWeaponUp();
        }
    }

    private void SelectWorldItem(GameObject item)
    {
        if (item)
        {
            if (item.CompareTag("Item"))
            {
                if (selectedItem != item)
                {
                    selectedItem = item;
                }
            }
            else
            {
                selectedItem = null;
            }
        }
        else
        {
            if (selectedItem)
            {
                selectedItem = null;
            }
        }
    }

    private void DropWeapon()
    {
        Rigidbody weaponRB = currentWeapon.GetComponent<Rigidbody>();
        weaponRB.isKinematic = false;
        weaponRB.AddRelativeForce(weaponThrowForce, 0, 0);
        weaponRB.AddRelativeTorque(UnityEngine.Random.onUnitSphere);
        currentWeapon.transform.parent = null;
        StartCoroutine(currentWeaponController.SetLayerAfterDelay(3F, 0));
        currentWeapon = null;
        currentWeaponController = null;
        SetAnimatorLayer("Unarmed");
    }

    private void PickWeaponUp()
    {
        if (selectedItem)
        {
            GrabWeapon(selectedItem);
            selectedItem = null;
        }
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

    private void GrabWeapon(GameObject weapon)
    {
        currentWeapon = weapon;
        currentWeapon.SetActive(true);
        //currentWeapon.GetComponent<PickableItem>().outline.enabled = false;
        Rigidbody weaponRB = currentWeapon.GetComponent<Rigidbody>();
        weaponRB.isKinematic = true;
        currentWeapon.transform.parent = rightHand.transform;
        currentWeapon.layer = 6; //player layer
        currentWeaponController = currentWeapon.GetComponent<WeaponController>();
        if (currentWeaponController.type == WeaponController.WeaponType.Rifle)
        {
            SetAnimatorLayer("Chemirail");
        }
        else if (currentWeaponController.type == WeaponController.WeaponType.Pistol)
        {
            SetAnimatorLayer("Laser Pistol");
        }
        PickableItem pickable = currentWeapon.GetComponent<PickableItem>();
        currentWeapon.transform.localPosition = pickable.relativePosition;
        currentWeapon.transform.localRotation = Quaternion.Euler(pickable.relativeRotation);
        weaponChangeEvent.Invoke(currentWeaponController);
    }

    private void SetAnimatorLayer(string name)
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

    private void PickWeaponFromInventory(int slot)
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
                DropWeapon();
            }
        }
        if (weapon)
        {
            //take weapon from selected slot
            inventoryChange.Invoke(slot, "");
            GrabWeapon(weapon);
            //print(String.Format("Taken weapon from inventory: {0}", weapon.name));
        }
    }

    public void PickWeapon1FromInventory(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            PickWeaponFromInventory(0);
        }
    }

    public void PickWeapon2FromInventory(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            PickWeaponFromInventory(1);
        }
    }

    public void PickWeapon3FromInventory(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            PickWeaponFromInventory(2);
        }
    }

    private void FixedUpdate()
    {
        if (cameraController)
        {
            SelectWorldItem(cameraController.targetItem);
        }
    }
}
