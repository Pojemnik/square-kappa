using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    private List<InventorySlot> smallSlots;
    private List<InventorySlot> bigSlots;
    private InventorySlot defaultWeaponSlot;
    private int totalSlots;
    private int maxSmallSlots;
    private int maxBigSlots;
    public int MeleWeaponSlotIndex => maxBigSlots + maxSmallSlots;

    public Inventory(int bigSlotsNumber, int smallSlotsNumber, GameObject startWeapon)
    {
        smallSlots = new List<InventorySlot>();
        bigSlots = new List<InventorySlot>();
        maxBigSlots = bigSlotsNumber;
        maxSmallSlots = smallSlotsNumber;
        defaultWeaponSlot = new InventorySlot(WeaponConfig.WeaponSize.Small);
        defaultWeaponSlot.AddWeapon(startWeapon);
    }

    public int AddWeapon(GameObject weapon)
    {
        WeaponConfig.WeaponSize size = weapon.GetComponent<WeaponConfig>().size;
        if (size == WeaponConfig.WeaponSize.Small)
        {
            if (smallSlots.Count < maxSmallSlots)
            {
                smallSlots.Add(new InventorySlot(WeaponConfig.WeaponSize.Small));
                smallSlots[smallSlots.Count - 1].AddWeapon(weapon);
                return smallSlots.Count - 1 + maxBigSlots;
            }
        }
        if (size == WeaponConfig.WeaponSize.Big)
        {
            if (bigSlots.Count < maxBigSlots)
            {
                bigSlots.Add(new InventorySlot(WeaponConfig.WeaponSize.Big));
                bigSlots[bigSlots.Count - 1].AddWeapon(weapon);
                return bigSlots.Count - 1;
            }
        }
        return -1;
    }

    //Returns a weapon or null if selected slot is empty/out of range
    public GameObject GetWeapon(int index)
    {
        if (maxBigSlots > index)
        {
            if (bigSlots.Count > index)
            {
                return bigSlots[index].Weapon;
            }
            return null;
        }
        index -= maxBigSlots;
        if (maxSmallSlots > index)
        {
            if (smallSlots.Count > index)
            {
                return smallSlots[index].Weapon;
            }
            return null;
        }
        index -= maxSmallSlots;
        if(index == 0)
        {
            return defaultWeaponSlot.Weapon;
        }
        return null;
    }
}

[System.Serializable]
public class InventorySlot
{
    public WeaponConfig.WeaponSize SlotSize => size;
    public GameObject Weapon => weaponObject;
    public WeaponController WeaponController => controllerObject;
    public bool Empty => weaponObject == null;

    private GameObject weaponObject;
    private WeaponController controllerObject;
    private WeaponConfig.WeaponSize size;

    public InventorySlot(WeaponConfig.WeaponSize weaponSize)
    {
        size = weaponSize;
    }

    public void AddWeapon(GameObject weapon)
    {
        if (Empty)
        {
            ProjectileWeaponController tempController = weapon.GetComponent<ProjectileWeaponController>();
            if (tempController == null)
            {
                throw new System.Exception("No weapon controller in weapon!");
            }
            if (tempController.Config.size != size)
            {
                throw new System.Exception("Wrong weapon size!");
            }
            weaponObject = weapon;
            controllerObject = tempController;
        }
        else
        {
            throw new System.Exception("Weapon slot already occupied");
        }
    }

    public void RemoveWeapon()
    {
        if (weaponObject == null)
        {
            Debug.LogWarning("Removing empty weapon from inventory!");
        }
        weaponObject = null;
        controllerObject = null;
    }
}
