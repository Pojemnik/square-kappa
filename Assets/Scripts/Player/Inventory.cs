using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    private List<InventorySlot> smallSlots;
    private List<InventorySlot> bigSlots;
    private InventorySlot defaultWeaponSlot;
    private int maxSmallSlots;
    private int maxBigSlots;
    public int MeleWeaponSlotIndex => maxBigSlots + maxSmallSlots;

    public Inventory(int bigSlotsNumber, int smallSlotsNumber, GameObject startWeapon)
    {
        maxBigSlots = bigSlotsNumber;
        maxSmallSlots = smallSlotsNumber;
        if (maxSmallSlots == 0)
        {
            smallSlots = new List<InventorySlot>();
        }
        else
        {
            smallSlots = Enumerable.Repeat(new InventorySlot(WeaponConfig.WeaponSlotType.Small), maxSmallSlots).ToList();
        }
        if (maxSmallSlots == 0)
        {
            bigSlots = new List<InventorySlot>();
        }
        else
        {
            bigSlots = Enumerable.Repeat(new InventorySlot(WeaponConfig.WeaponSlotType.Big), maxBigSlots).ToList();
        }
        defaultWeaponSlot = new InventorySlot(WeaponConfig.WeaponSlotType.Mele);
        defaultWeaponSlot.AddWeapon(startWeapon);
    }

    public int AddWeapon(GameObject weapon)
    {
        WeaponConfig.WeaponSlotType type = weapon.GetComponent<WeaponController>().Config.slotType;
        if (type == WeaponConfig.WeaponSlotType.Small)
        {
            int slotIndex = FindFirstEmptySlot(smallSlots);
            if (slotIndex != -1)
            {
                smallSlots[slotIndex].AddWeapon(weapon);
                return slotIndex + bigSlots.Count;
            }
        }
        if (type == WeaponConfig.WeaponSlotType.Big)
        {
            int slotIndex = FindFirstEmptySlot(bigSlots);
            if (slotIndex != -1)
            {
                bigSlots[slotIndex].AddWeapon(weapon);
                return slotIndex;
            }
        }
        return -1;
    }

    public void ReplaceWeapon(int index, GameObject weapon)
    {
        InventorySlot slot = GetSlotOfIndex(index);
        slot.RemoveWeapon();
        slot.AddWeapon(weapon);
    }

    //Returns a weapon or null if selected slot is empty/out of range
    public GameObject GetWeapon(int index)
    {
        return GetSlotOfIndex(index).Weapon;
    }

    public bool SlotAvailable(WeaponConfig.WeaponSlotType type)
    {
        int slotIndex = -1;
        if (type == WeaponConfig.WeaponSlotType.Small)
        {
            slotIndex = FindFirstEmptySlot(smallSlots);
        }
        else if (type == WeaponConfig.WeaponSlotType.Big)
        {
            slotIndex = FindFirstEmptySlot(bigSlots);
        }
        return slotIndex != -1;
    }

    public void RemoveWeapon(int index)
    {
        InventorySlot slot = GetSlotOfIndex(index);
        slot.RemoveWeapon();
    }

    public void Clear()
    {
        foreach(InventorySlot slot in smallSlots)
        {
            slot.RemoveWeapon();
        }
        foreach (InventorySlot slot in bigSlots)
        {
            slot.RemoveWeapon();
        }
        defaultWeaponSlot.RemoveWeapon();
    }

    public int GetSlotWithWeapon()
    {
        for(int i = 0; i < bigSlots.Count + smallSlots.Count + 1; i++)
        {
            if(!GetSlotOfIndex(i).Empty)
            {
                return i;
            }
        }
        throw new System.Exception("No weapon found in any slot. This should never happen");
    }

    private InventorySlot GetSlotOfIndex(int index)
    {
        if (maxBigSlots > index)
        {
            if (bigSlots.Count > index)
            {
                return bigSlots[index];
            }
            return null;
        }
        index -= maxBigSlots;
        if (maxSmallSlots > index)
        {
            if (smallSlots.Count > index)
            {
                return smallSlots[index];
            }
            return null;
        }
        index -= maxSmallSlots;
        if (index == 0)
        {
            return defaultWeaponSlot;
        }
        return null;
    }

    private int FindFirstEmptySlot(List<InventorySlot> slots)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].Empty)
            {
                return i;
            }
        }
        return -1;
    }
}

[System.Serializable]
public class InventorySlot
{
    public WeaponConfig.WeaponSlotType SlotType => type;
    public GameObject Weapon => weaponObject;
    public WeaponController WeaponController => controllerObject;
    public bool Empty => weaponObject == null;

    private GameObject weaponObject;
    private WeaponController controllerObject;
    private readonly WeaponConfig.WeaponSlotType type;

    public InventorySlot(WeaponConfig.WeaponSlotType weaponSize)
    {
        type = weaponSize;
    }

    public void AddWeapon(GameObject weapon)
    {
        if (Empty)
        {
            WeaponController tempController = weapon.GetComponent<WeaponController>();
            if (tempController == null)
            {
                throw new System.Exception("No weapon controller in weapon!");
            }
            if (tempController.Config.slotType != type)
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
            Debug.LogWarning("Removing empty weapon from inventory slot!");
        }
        weaponObject = null;
        controllerObject = null;
    }
}
