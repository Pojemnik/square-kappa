using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(ItemChanger))]
public class PlayerInventoryController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Unit owner;

    [Header("Inventory config")]
    [SerializeField]
    private int smallSlots;
    [SerializeField]
    private int bigSlots;

    private ItemChanger changer;
    private Inventory inventory;
    private int currentSlot;

    private void Start()
    {
        inventory = new Inventory(smallSlots, bigSlots, owner.CurrentWeapon);
        currentSlot = inventory.MeleWeaponSlotIndex;
    }

    public void SelectSlot(int slot)
    {
        GameObject weapon = inventory.GetWeapon(slot);
        if (owner.CurrentWeapon != null)
        {
            //some weapon currently in hands
            int insertedSlot = inventory.AddWeapon(owner.CurrentWeapon);
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
