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

    private DisplayController totalAmmoDisplayController;
    private DisplayController currentAmmoDisplayController;
    private WeaponController weaponController;

    private void Start()
    {
        totalAmmoDisplayController = totalAmmoDisplay.GetComponent<DisplayController>();
        currentAmmoDisplayController = currentAmmoDisplay.GetComponent<DisplayController>();
    }

    public void OnWeaponChange(WeaponController controller)
    {
        if(controller.Config.type == WeaponConfig.WeaponType.Rifle)
        {
            DisplayAmmo();
        }
        else
        {
            HideAmmo();
        }
        if (weaponController != null)
        {
            weaponController.AmmoChangeEvent.RemoveListener(OnAmmoAmountUpdated);
        }
        weaponController = controller;
        weaponController.AmmoChangeEvent.AddListener(OnAmmoAmountUpdated);

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
