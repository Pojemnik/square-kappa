using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputAdapter : MonoBehaviour
{
    private float defaultCameraSensitivity;
    private float zoomCameraSensitivity;
    [Header("References")]
    public UnitController owner;

    private bool zoom;

    public void MoveXZ(InputAction.CallbackContext context)
    {
        owner.movement.MoveXZ(context.ReadValue<Vector2>());
    }

    public void MoveY(InputAction.CallbackContext context)
    {
        owner.movement.MoveY(context.ReadValue<float>());
    }

    public void Roll(InputAction.CallbackContext context)
    {
        owner.movement.Roll(context.ReadValue<float>());
    }

    public void RelativeLook(InputAction.CallbackContext context)
    {
        Vector2 rawInputLook = context.ReadValue<Vector2>();
        if (zoom)
        {
            owner.movement.RelativeLook(rawInputLook * zoomCameraSensitivity);
        }
        else
        {
            owner.movement.RelativeLook(rawInputLook * defaultCameraSensitivity);
        }
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            owner.shooting.StartFire();
        }
        else if (context.canceled)
        {
            owner.shooting.StopFire();
        }
    }

    public void DropItem(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        owner.itemChanger.DropCurrentWeapon();
    }

    public void PickItem(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        owner.itemChanger.PickOrSwapWeapon();
    }

    public void Reload(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (owner.CurrentWeaponController != null)
            {
                owner.shooting.Reload();
            }
        }
    }

    public void OnZoomEnable()
    {
        zoom = true;
    }

    public void OnZoomDisble()
    {
        zoom = false;
    }

    public void ChangeWeapon(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            owner.itemChanger.ChangeWeapon((int)context.ReadValue<float>());
        }
    }

    private void Awake()
    {
        ZoomController zoomController = FindObjectOfType<ZoomController>();
        zoomController.zoomEnabled.AddListener(OnZoomEnable);
        zoomController.zoomDisabled.AddListener(OnZoomDisble);
    }

    private void Start()
    {
        defaultCameraSensitivity = SettingsManager.Instance.MouseSensitivity.Value;
        SettingsManager.Instance.MouseSensitivity.ValueChanged += (_, val) => { defaultCameraSensitivity = val; };
        zoomCameraSensitivity = SettingsManager.Instance.ZoomMouseSensitivity.Value;
        SettingsManager.Instance.ZoomMouseSensitivity.ValueChanged += (_, val) => { zoomCameraSensitivity = val; };
    }
}
