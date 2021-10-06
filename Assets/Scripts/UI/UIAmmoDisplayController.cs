using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAmmoDisplayController : MonoBehaviour
{
    [SerializeField]
    private GameObject totalAmmoDisplay;
    [SerializeField]
    private GameObject currentAmmoDisplay;
    [SerializeField]
    private GameObject displayContainer;
    [SerializeField]
    private SerializableDictionary<WeaponConfig.WeaponType, UnityEngine.Sprite> weaponIcons;
    [SerializeField]
    private UnityEngine.UI.Image weaponImage;

    private DisplayController totalAmmoDisplayController;
    private DisplayController currentAmmoDisplayController;
    private WeaponController weaponController;

    private void Start()
    {
        totalAmmoDisplayController = totalAmmoDisplay.GetComponent<DisplayController>();
        currentAmmoDisplayController = currentAmmoDisplay.GetComponent<DisplayController>();
        EventManager.Instance.AddListener("Victory", HideAmmo);
        EventManager.Instance.AddListener("PlayerDeath", HideAmmo);
    }

    public void OnWeaponChange(WeaponController controller)
    {
        if (controller.Config.type == WeaponConfig.WeaponType.Fists)
        {
            HideAmmo();
        }
        else
        {
            DisplayAmmo();
            if (weaponIcons.ContainsKey(controller.Config.type))
            {
                weaponImage.sprite = weaponIcons[controller.Config.type];
            }
        }
        if (weaponController != null)
        {
            weaponController.ammoChangeEvent -= (s, a) => OnAmmoAmountUpdated(a);
        }
        weaponController = controller;
        weaponController.ammoChangeEvent += (s, a) => OnAmmoAmountUpdated(a);

    }

    private void HideAmmo()
    {
        totalAmmoDisplay.SetActive(false);
        currentAmmoDisplay.SetActive(false);
        displayContainer.SetActive(false);
    }

    private void DisplayAmmo()
    {
        totalAmmoDisplay.SetActive(true);
        currentAmmoDisplay.SetActive(true);
        displayContainer.SetActive(true);
    }

    private void OnAmmoAmountUpdated((int current, int total) ammo)
    {
        currentAmmoDisplayController.SetValue(ammo.current);
        totalAmmoDisplayController.SetValue(ammo.total);
    }
}
