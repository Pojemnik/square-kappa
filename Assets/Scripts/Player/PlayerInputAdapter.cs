using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputAdapter : MonoBehaviour
{
    [SerializeField]
    private float scrollSensitivity;
    [Header("References")]
    public UnitController owner;

    private bool zoom;
    private float cameraSensitivity;
    private float zoomCameraSensitivity;
    private bool enableMovement = false;
    private bool slowingDown;

    public void MoveXZ(InputAction.CallbackContext context)
    {
        if (!enableMovement)
        {
            return;
        }
        owner.movement.MoveXZ(context.ReadValue<Vector2>());
    }

    public void MoveY(InputAction.CallbackContext context)
    {
        if (!enableMovement)
        {
            return;
        }
        owner.movement.MoveY(context.ReadValue<float>());
    }

    public void Roll(InputAction.CallbackContext context)
    {
        if (!enableMovement)
        {
            return;
        }
        owner.movement.Roll(context.ReadValue<float>());
    }

    public void RelativeLook(InputAction.CallbackContext context)
    {
        if (!enableMovement)
        {
            return;
        }
        Vector2 rawInputLook = context.ReadValue<Vector2>();
        if (zoom)
        {
            owner.movement.RelativeLook(rawInputLook * zoomCameraSensitivity);
        }
        else
        {
            owner.movement.RelativeLook(rawInputLook * cameraSensitivity);
        }
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (!enableMovement)
        {
            return;
        }
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
        if (!enableMovement)
        {
            return;
        }
        if (!context.started)
        {
            return;
        }
        owner.itemChanger.DropCurrentWeapon();
    }

    public void Action1(InputAction.CallbackContext context)
    {
        if (!enableMovement)
        {
            return;
        }
        if (!context.started)
        {
            return;
        }
        owner.itemChanger.PickOrSwapWeapon();
        owner.interactor.InteractWithSelected();
    }

    public void Reload(InputAction.CallbackContext context)
    {
        if (!enableMovement)
        {
            return;
        }
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
        if (!enableMovement)
        {
            return;
        }
        if (context.started)
        {
            owner.itemChanger.ChangeWeapon((int)context.ReadValue<float>());
        }
    }

    public void NextWeapon(InputAction.CallbackContext context)
    {
        if (!enableMovement)
        {
            return;
        }
        if (context.started)
        {
            owner.itemChanger.NextWeapon((int)(context.ReadValue<Vector2>().y * scrollSensitivity));
        }
    }

    public void SlowDownCallback(InputAction.CallbackContext context)
    {
        if (!enableMovement)
        {
            return;
        }
        if(context.started)
        {
            slowingDown = true;
        }
        if(context.canceled)
        {
            slowingDown = false;
        }
    }

    private void SlowDown()
    {
        owner.movement.SlowDown();
    }

    private void Awake()
    {
        ZoomController zoomController = FindObjectOfType<ZoomController>();
        zoomController.zoomEnabled.AddListener(OnZoomEnable);
        zoomController.zoomDisabled.AddListener(OnZoomDisble);
    }

    private void Start()
    {
        cameraSensitivity = SettingsManager.Instance.MouseSensitivity.Value;
        SettingsManager.Instance.MouseSensitivity.ValueChanged += (_, val) => { cameraSensitivity = val; };
        zoomCameraSensitivity = SettingsManager.Instance.ZoomMouseSensitivity.Value;
        SettingsManager.Instance.ZoomMouseSensitivity.ValueChanged += (_, val) => { zoomCameraSensitivity = val; };
        EventManager.Instance.AddListener("GameStart", () => enableMovement = true);
    }

    private void Update()
    {
        if(slowingDown)
        {
            SlowDown();
        }
    }
}
