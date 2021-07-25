using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ZoomController : MonoBehaviour
{
    [SerializeField]
    private float defaultFov;
    [SerializeField]
    private float zoomFov;

    [HideInInspector]
    public UnityEvent zoomEnabled;
    [HideInInspector]
    public UnityEvent zoomDisabled;

    private bool zoomState = false;

    public void ChangeZoomState()
    {
        zoomState = !zoomState;
        if(zoomState)
        {
            EnableZoom();
        }
        else
        {
            DisableZoom();
        }
    }

    public void EnableZoom()
    {
        Camera.main.fieldOfView = zoomFov;
        zoomEnabled.Invoke();
    }

    public void DisableZoom()
    {
        Camera.main.fieldOfView = defaultFov;
        zoomDisabled.Invoke();
    }
}
