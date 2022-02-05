using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentInteractor : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Unit owner;
    [SerializeField]
    private InteractionTipController interactionTip;

    [Header("Camera")]
    [SerializeField]
    private GameObject firstPresonCamera;

    private PlayerCameraController cameraController;
    private GameObject selectedObject;

    private void Awake()
    {
        cameraController = firstPresonCamera.GetComponent<PlayerCameraController>();
    }

    private void Update()
    {
        if (cameraController != null)
        {
            SelectWorldObject(cameraController.targetItem);
        }
    }

    private bool IsScreenPointInViewport(Vector3 screenPos)
    {
        bool onScreenX = screenPos.x > 0 && screenPos.x < Camera.main.pixelWidth;
        bool onScreenY = screenPos.y > 0 && screenPos.y < Camera.main.pixelHeight;
        bool onScreenZ = screenPos.z > 0;
        return onScreenX && onScreenY && onScreenZ;
    }

    private void SelectWorldObject(GameObject selection)
    {
        if (selectedObject != null && selectedObject == selection)
        {
            //Update tip position
            Vector3 screenPos = Camera.main.WorldToScreenPoint(selection.transform.position);
            interactionTip.DisplayTip(screenPos, InteractionTipController.TipType.Interact);
            return;
        }
        selectedObject = selection;
        if (selection == null)
        {
            interactionTip.HideTip(InteractionTipController.TipType.Interact);
        }
        else
        {
            if (selection.CompareTag("Interactive"))
            {
                if (selection.GetComponent<IInteractive>() == null)
                {
                    Debug.LogErrorFormat("Error. Selecteed object {0} tagged interactive, but without interactive component.", selection.name);
                    interactionTip.HideTip(InteractionTipController.TipType.Interact);
                    return;
                }
                Vector3 screenPos = Camera.main.WorldToScreenPoint(selection.transform.position);
                if (IsScreenPointInViewport(screenPos))
                {
                    interactionTip.DisplayTip(screenPos, InteractionTipController.TipType.Interact);
                }
                else
                {
                    interactionTip.HideTip(InteractionTipController.TipType.Interact);
                }
            }
            else
            {
                selectedObject = null;
                interactionTip.HideTip(InteractionTipController.TipType.Interact);
            }
        }
    }

    public void InteractWithSelected()
    {
        selectedObject?.GetComponent<IInteractive>().Interact();
    }
}
