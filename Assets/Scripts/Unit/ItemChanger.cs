using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitShooting))]
[RequireComponent(typeof(Collider))]
public class ItemChanger : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Unit owner;

    [Header("Default weapon")]
    [SerializeField]
    private GameObject defaultWeapon;

    [Header("Camera")]
    [SerializeField]
    private GameObject firstPresonCamera;

    [Header("Item pickup")]
    [SerializeField]
    private float weaponPickupRange;
    [SerializeField]
    private GameObject weaponMountingPoint;

    [Header("Item dropping")]
    [SerializeField]
    private float weaponThrowForce;
    [SerializeField]
    private float weaponDropCollisionTimeout;

    [Header("Inventory config")]
    [SerializeField]
    private int smallSlots;
    [SerializeField]
    private int bigSlots;

    [Header("Events")]
    public UnityEngine.Events.UnityEvent<WeaponController> weaponChangeEvent;
    public UnityEngine.Events.UnityEvent<WeaponController> weaponInCurrentSlotChangeEvent;
    public UnityEngine.Events.UnityEvent<int> currentSlotChangeEvent;

    private WeaponController defaultWeaponController;
    private PlayerCameraController cameraController;
    private GameObject selectedItem;
    private new Rigidbody rigidbody;
    private UnitShooting shooting;
    private event System.EventHandler<PickableItem> targetChanged;
    private Inventory inventory;
    private int currentSlot;

    private void SelectWorldItem(GameObject item)
    {
        GameObject lastItem = selectedItem;
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
        if (lastItem != selectedItem)
        {
            targetChanged?.Invoke(this, selectedItem?.GetComponent<PickableItem>());
        }
    }

    public void ClearInventory()
    {
        inventory.Clear();
    }

    public void PickOrSwapWeapon()
    {
        if (selectedItem == null)
        {
            return;
        }
        WeaponController selectedWeaponController = selectedItem.GetComponent<WeaponController>();
        if (selectedWeaponController == null)
        {
            return;
        }
        if (owner.CurrentWeapon == null)
        {
            Debug.LogError("Current weapon is null in player. This should never happen");
        }
        if (owner.CurrentWeaponController.Config.slotType == selectedWeaponController.Config.slotType)
        {
            if (inventory.SlotAvailable(selectedWeaponController.Config.slotType))
            {
                //Pick up to slot and select
                int slot = inventory.AddWeapon(selectedItem);
                ChangeActiveSlot(slot);
                weaponInCurrentSlotChangeEvent.Invoke(owner.CurrentWeaponController);
            }
            else
            {
                //swap in slot
                inventory.ReplaceWeapon(currentSlot, selectedItem);
                ChangeActiveSlot(currentSlot);
                weaponInCurrentSlotChangeEvent.Invoke(owner.CurrentWeaponController);
            }
            selectedItem = null;
        }
        else
        {
            if (inventory.SlotAvailable(selectedWeaponController.Config.slotType))
            {
                //Pick up to slot and select
                int slot = inventory.AddWeapon(selectedItem);
                ChangeActiveSlot(slot);
                weaponInCurrentSlotChangeEvent.Invoke(owner.CurrentWeaponController);
                selectedItem = null;
            }
            else
            {
                //???
                Debug.LogWarning("IDK");
            }
        }
    }

    public void ChangeActiveSlot(int slot)
    {
        owner.CurrentWeaponController?.StopAttack();
        owner.CurrentWeapon?.SetActive(false);
        GameObject weaponFromInventory = inventory.GetWeapon(slot);
        if(weaponFromInventory == null)
        {
            Debug.LogFormat("Selected weapon from slot {0}, but it's empty.", slot);
            return;
        }
        Debug.LogFormat("Selected weapon {0} from slot {1}", weaponFromInventory, slot);
        BindWeapon(weaponFromInventory);
        currentSlotChangeEvent.Invoke(slot);
        currentSlot = slot;
    }

    public void ThrowWeaponAway()
    {
        GameObject currentWeapon = owner.CurrentWeapon;
        if (currentWeapon == defaultWeapon)
        {
            return;
        }
        inventory.RemoveWeapon(currentSlot);
        StartCoroutine(currentWeapon.GetComponent<PickableItem>().SetLayerAfterDelay(weaponDropCollisionTimeout, 0));
        Rigidbody weaponRB = currentWeapon.GetComponent<Rigidbody>();
        UnbindWeapon(weaponRB);
        weaponRB.AddRelativeForce(0, 0, weaponThrowForce);
        weaponRB.AddRelativeTorque(0, -20, 0);
        weaponRB.AddForce(rigidbody.velocity, ForceMode.VelocityChange);
        ChangeActiveSlot(inventory.GetSlotWithWeapon());
    }

    private void UnbindWeapon(Rigidbody weaponRB)
    {
        weaponRB.isKinematic = false;
        owner.CurrentWeapon.transform.parent = null;
        owner.CurrentWeaponController.StopAttack();
        int ammoLeft = owner.CurrentWeaponController.Reload(-1);
        shooting.PickUpAmmo(owner.CurrentWeaponController.Config.type, ammoLeft);
        owner.CurrentWeapon.GetComponent<PickableItem>().OnDrop();
        SceneLoadingManager.Instance.AddObjectToRemoveOnReload(owner.CurrentWeapon);
        BindWeapon(defaultWeapon);
    }

    private void BindWeapon(GameObject weapon)
    {
        owner.CurrentWeapon = weapon;
        owner.CurrentWeapon.SetActive(true);
        Rigidbody weaponRB = owner.CurrentWeapon.GetComponent<Rigidbody>();
        weaponRB.isKinematic = true;
        owner.CurrentWeapon.transform.parent = weaponMountingPoint.transform;
        owner.CurrentWeapon.layer = 6; //player layer
        PickableItem pickable = owner.CurrentWeapon.GetComponent<PickableItem>();
        pickable.OnPickup();
        owner.CurrentWeapon.transform.localPosition = pickable.relativePosition;
        owner.CurrentWeapon.transform.localRotation = Quaternion.Euler(pickable.relativeRotation);
        weaponChangeEvent.Invoke(owner.CurrentWeaponController);
        shooting.ChangeWeaponController(owner.CurrentWeaponController);
    }

    public void DropAndDestroyWeapon()
    {
        GameObject weapon = owner.CurrentWeapon;
        if (weapon != null && weapon != defaultWeapon)
        {
            inventory.RemoveWeapon(currentSlot);
            UnbindWeapon(owner.CurrentWeapon.GetComponent<Rigidbody>());
            Destroy(weapon);
        }
    }

    private void Awake()
    {
        cameraController = firstPresonCamera.GetComponent<PlayerCameraController>();
        rigidbody = GetComponent<Rigidbody>();
        shooting = GetComponent<UnitShooting>();
    }

    private void Start()
    {
        cameraController.targettingRange = weaponPickupRange;
        if (defaultWeapon == null)
        {
            throw new System.Exception("No default weapon");
        }
        defaultWeaponController = defaultWeapon.GetComponent<WeaponController>();
        if (defaultWeaponController == null)
        {
            throw new System.Exception("No controller in default weapon");
        }
        BindWeapon(defaultWeapon);
        targetChanged += ItemsManager.Instance.OnItemTargeted;
        inventory = new Inventory(smallSlots, bigSlots, owner.CurrentWeapon);
        currentSlot = inventory.MeleWeaponSlotIndex;
    }

    private void Update()
    {
        if (cameraController)
        {
            SelectWorldItem(cameraController.targetItem);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ammo"))
        {
            AmmoBoxController ammoBox = other.gameObject.GetComponent<AmmoBoxController>();
            foreach (SerializableDictionary<WeaponConfig.WeaponType, int>.Pair ammo in ammoBox.ammoCount)
            {
                shooting.PickUpAmmo(ammo.Key, ammo.Value);
            }
        }
    }

}
