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
            smallSlots = InitSlots(WeaponConfig.WeaponSlotType.Small, maxSmallSlots);
        }
        if (maxSmallSlots == 0)
        {
            bigSlots = new List<InventorySlot>();
        }
        else
        {
            bigSlots = InitSlots(WeaponConfig.WeaponSlotType.Big, maxBigSlots);
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

    public void AddWeaponToSlot(int index, GameObject weapon)
    {
        InventorySlot slot = GetSlotOfIndex(index);
        if(!slot.Empty)
        {
            throw new System.Exception(string.Format("Tried to add weapon {0} to occupied slot (slot index {1})", weapon.gameObject.name, index));
        }
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

    public int GetSlotOfSize(WeaponConfig.WeaponSlotType type)
    {
        switch (type)
        {
            case WeaponConfig.WeaponSlotType.Small:
                return maxBigSlots;
            case WeaponConfig.WeaponSlotType.Big:
                return 0;
            default:
                return maxBigSlots + maxSmallSlots;
        }

    }

    private List<InventorySlot> InitSlots(WeaponConfig.WeaponSlotType type, int count)
    {
        List<InventorySlot> slots = new List<InventorySlot>();
        for(int i = 0; i < count; i++)
        {
            slots.Add(new InventorySlot(type));
        }
        return slots;
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
