using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomController : MonoBehaviour
{
    [SerializeField]
    private float defaultFov;
    [SerializeField]
    private float zoomFov;

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
    }

    public void DisableZoom()
    {
        Camera.main.fieldOfView = defaultFov;
    }
}
