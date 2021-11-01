using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplayController : MonoBehaviour
{
    [SerializeField]
    private List<TMPro.TextMeshProUGUI> slotDescriptions;
    [SerializeField]
    private List<WeaponConfig.WeaponSlotType> slotSizes;
    [SerializeField]
    private int defaultSlot;

    private List<string> weaponNames = new List<string> { "Empty", "Empty", "Empty", "Fists" };
    private int currentSlot;

    private Dictionary<WeaponConfig.WeaponSlotType, string> slotSizeNames = new Dictionary<WeaponConfig.WeaponSlotType, string>
    {
        {WeaponConfig.WeaponSlotType.Big, "big" },
        {WeaponConfig.WeaponSlotType.Small, "small"},
        {WeaponConfig.WeaponSlotType.Mele, "mele"}
    };

    private void Awake()
    {
        currentSlot = defaultSlot;
        for (int i = 0; i < slotDescriptions.Count; i++)
        {
            slotDescriptions[i].text = GetSlotText(i);
        }
    }

    private string GetSlotText(int slotIndex)
    {
        string sizeName = slotSizeNames[slotSizes[slotIndex]];
        string selectedStr = slotIndex == currentSlot ? "* " : "";
        return string.Format("{2} {0}: {3} ({1} weapon)", slotIndex, sizeName, selectedStr, weaponNames[slotIndex]);
    }

    public void OnChangeCurrentSlotContent(WeaponController weapon)
    {
        if (weapon == null)
        {
            weaponNames[currentSlot] = "Empty";
        }
        else
        {
            weaponNames[currentSlot] = weapon.gameObject.name;
        }
        slotDescriptions[currentSlot].text = GetSlotText(currentSlot);
    }

    public void OnChangeSelectedSlot(int selectedSlot)
    {
        int lastSelected = currentSlot;
        currentSlot = selectedSlot;
        slotDescriptions[lastSelected].text = GetSlotText(lastSelected);
        slotDescriptions[currentSlot].text = GetSlotText(currentSlot);
    }
}
