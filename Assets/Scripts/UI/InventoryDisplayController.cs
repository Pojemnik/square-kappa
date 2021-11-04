using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplayController : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Image[] weaponImages;
    [SerializeField]
    private GameObject[] backgroundPanels;
    [SerializeField]
    private int defaultSlot;

    private int currentSlot;

    private void Awake()
    {
        currentSlot = defaultSlot;
        foreach(GameObject panel in backgroundPanels)
        {
            panel.SetActive(false);
        }
        backgroundPanels[currentSlot].SetActive((true));
    }

    public void OnChangeCurrentSlotContent(WeaponController weapon)
    {
        if (weapon == null)
        {
            weaponImages[currentSlot].sprite = UIAssetsManager.Instance.GetEmptySprite();
        }
        else
        {
            weaponImages[currentSlot].sprite = UIAssetsManager.Instance.GetWeaponSprite(weapon.Config.type);
        }
    }

    public void OnChangeSelectedSlot(int selectedSlot)
    {
        backgroundPanels[currentSlot].SetActive(false);
        currentSlot = selectedSlot;
        backgroundPanels[currentSlot].SetActive(true);

    }
}
