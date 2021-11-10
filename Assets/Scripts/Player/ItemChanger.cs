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
        if (selectedItem == item)
        {
            return;
        }
        if (item == null)
        {
            selectedItem = null;
        }
        else
        {
            if (item.CompareTag("Item"))
            {
                selectedItem = item;
            }
            else
            {
                selectedItem = null;
            }
        }
        targetChanged?.Invoke(this, selectedItem?.GetComponent<PickableItem>());
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
                if (owner.CurrentWeapon == defaultWeapon)
                {
                    return;
                }
                ThrowWeapon();
                inventory.AddWeaponToSlot(currentSlot, selectedItem);
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
                int slot = inventory.GetSlotOfSize(selectedWeaponController.Config.slotType);
                ChangeActiveSlot(slot);
                ThrowWeapon();
                inventory.AddWeaponToSlot(currentSlot, selectedItem);
                ChangeActiveSlot(currentSlot);
                weaponInCurrentSlotChangeEvent.Invoke(owner.CurrentWeaponController);
            }
        }
    }

    private void ChangeActiveSlot(int slot)
    {
        GameObject weaponFromInventory = inventory.GetWeapon(slot);
        if (weaponFromInventory == null)
        {
            //Debug.LogFormat("Selected weapon from slot {0}, which is empty", slot);
            return;
        }
        owner.CurrentWeaponController?.StopAttack();
        owner.CurrentWeapon?.SetActive(false);
        //Debug.LogFormat("Selected weapon {0} from slot {1}", weaponFromInventory, slot);
        BindWeapon(weaponFromInventory);
        currentSlotChangeEvent.Invoke(slot);
        currentSlot = slot;
    }

    public void ChangeWeapon(int slot)
    {
        if (slot == currentSlot)
        {
            //Debug.LogFormat("Selected weapon from slot {0}, which is the same as current", slot);
            return;
        }
        ChangeActiveSlot(slot);
    }

    public void NextWeapon(int delta)
    {
        if (delta == 0)
        {
            return;
        }
        int slot;
        if (delta > 0)
        {
            slot = inventory.GetNextWeapon(currentSlot);
        }
        else
        {
            slot = inventory.GetPreviousWeapon(currentSlot);
        }
        if (slot == -1)
        {
            return;
        }
        ChangeWeapon(slot);
    }

    public void ThrowWeapon()
    {
        GameObject currentWeapon = owner.CurrentWeapon;
        inventory.RemoveWeapon(currentSlot);
        weaponInCurrentSlotChangeEvent.Invoke(null);
        StartCoroutine(currentWeapon.GetComponent<PickableItem>().SetLayerAfterDelay(weaponDropCollisionTimeout, 0));
        Rigidbody weaponRB = currentWeapon.GetComponent<Rigidbody>();
        UnbindWeapon(weaponRB);
        weaponRB.AddRelativeForce(0, 0, weaponThrowForce);
        weaponRB.AddRelativeTorque(0, -20, 0);
        weaponRB.AddForce(rigidbody.velocity, ForceMode.VelocityChange);
    }

    public void DropCurrentWeapon()
    {
        if (owner.CurrentWeapon == defaultWeapon)
        {
            return;
        }
        ThrowWeapon();
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
        inventory = new Inventory(bigSlots, smallSlots, owner.CurrentWeapon);
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
