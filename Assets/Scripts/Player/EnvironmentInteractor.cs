using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentInteractor : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Unit owner;

    [Header("Camera")]
    [SerializeField]
    private GameObject firstPresonCamera;

    private PlayerCameraController cameraController;
    private InteractiveObject selectedObject;

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

    private void SelectWorldObject(GameObject selection)
    {
        if (selectedObject != null && selectedObject.gameObject == selection)
        {
            return;
        }
        if (selection == null)
        {
            selectedObject = null;
        }
        else
        {
            if (selection.CompareTag("Interactive"))
            {
                selectedObject = selection.GetComponent<InteractiveObject>();
                if(selectedObject == null)
                {
                    Debug.LogErrorFormat("Error. Selecteed object {0} tagged interactive, but without interactive component.", selection.name);
                }
            }
            else
            {
                selectedObject = null;
            }
        }
    }

    public void InteractWithSelected()
    {
        selectedObject?.Interact();
    }
}
