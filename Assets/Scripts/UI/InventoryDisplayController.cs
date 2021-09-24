using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplayController : MonoBehaviour
{
    public List<TMPro.TextMeshProUGUI> slotDescriptions;
    public List<ProjectileWeaponConfig.WeaponSize> slotSizes;

    private void Awake()
    {
        for(int i = 0; i < slotDescriptions.Count; i++)
        {
            slotDescriptions[i].text = GetDefaultText(i, slotSizes[i]);
        }
    }

    private string GetDefaultText(int slotNumber, ProjectileWeaponConfig.WeaponSize size)
    {
        string sizeName = (slotSizes[slotNumber] == ProjectileWeaponConfig.WeaponSize.Small) ? "small" : "big";
        return string.Format("{0}: Empty ({1} weapon)", slotNumber, sizeName);
    }

    public void AddWeapon(int slot, string weaponName)
    {
        print(string.Format("Slot: {0} Weapon: {1}", slot, weaponName));
        if (weaponName == "")
        {
            slotDescriptions[slot].text = GetDefaultText(slot, slotSizes[slot]);
        }
        else
        {
            slotDescriptions[slot].text = string.Format("{0}: {1}", slot, weaponName);
        }
    }
}
