using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputAdapter : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private EnemyMarkersController enemyMarkersController;
    [SerializeField]
    private ZoomController zoomController;
    [SerializeField]
    private PauseController pauseController;

    public void ChangeEnemyMarkersDisplayMode(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            enemyMarkersController.ChangeDisplayMode();
        }
    }

    public void ChangeZoomState(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            zoomController.EnableZoom();
        }
        if(context.canceled)
        {
            zoomController.DisableZoom();
        }
    }

    public void ChangePauseState(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            pauseController.ChangePauseState();
        }
    }
}
