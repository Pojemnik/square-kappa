using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    private List<InventorySlot> smallSlots;
    private List<InventorySlot> bigSlots;
    private int totalSlots;
    private int maxSmallSlots;
    private int maxBigSlots;

    public Inventory(int bigSlotsNumber, int smallSlotsNumber)
    {
        smallSlots = new List<InventorySlot>();
        bigSlots = new List<InventorySlot>();
        maxBigSlots = bigSlotsNumber;
        maxSmallSlots = smallSlotsNumber;
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

    public GameObject GetWeapon(int index)
    {
        if (maxBigSlots > index)
        {
            if (bigSlots.Count > index)
            {
                GameObject weapon = bigSlots[index].weapon;
                bigSlots.RemoveAt(index);
                return weapon;
            }
            return null;
        }
        index -= maxBigSlots;
        if (maxSmallSlots > index)
        {
            if (smallSlots.Count > index)
            {
                GameObject weapon = smallSlots[index].weapon;
                smallSlots.RemoveAt(index);
                return weapon;
            }
            return null;
        }
        return null;
    }
}

[System.Serializable]
public class InventorySlot
{
    public WeaponConfig.WeaponSize slotSize { get { return size; } }
    public GameObject weapon { get { return weaponObject; } }
    public WeaponController controller { get { return controllerObject; } }
    public bool empty { get { return weaponObject == null; } }

    private GameObject weaponObject;
    private WeaponController controllerObject;
    private WeaponConfig.WeaponSize size;

    public InventorySlot(WeaponConfig.WeaponSize weaponSize)
    {
        size = weaponSize;
    }

    public void AddWeapon(GameObject weapon)
    {
        if (empty)
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
