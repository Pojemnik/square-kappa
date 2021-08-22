using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputAdapter : MonoBehaviour
{
    [SerializeField]
    private float defaultCameraSensitivity;
    [SerializeField]
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
        owner.DropItem();
    }

    public void PickItem(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        owner.PickItem();
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            owner.StartDash();
        }
        if (context.canceled)
        {
            owner.CancelDash();
        }
    }

    public void PickWeapon1FromInventory(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            owner.PickWeaponFromInventory(0);
        }
    }

    public void PickWeapon2FromInventory(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            owner.PickWeaponFromInventory(1);
        }
    }

    public void PickWeapon3FromInventory(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            owner.PickWeaponFromInventory(2);
        }
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

    private void Awake()
    {
        ZoomController zoomController = FindObjectOfType<ZoomController>();
        zoomController.zoomEnabled.AddListener(OnZoomEnable);
        zoomController.zoomDisabled.AddListener(OnZoomDisble);
    }
}
