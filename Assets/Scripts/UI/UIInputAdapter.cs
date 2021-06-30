using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputAdapter : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject enemyMarkersControllerObject;

    private EnemyMarkersController enemyMarkersController;
    public void ChangeEnemyMarkersDisplayMode(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            enemyMarkersController.ChangeDisplayMode();
        }
    }

    private void Awake()
    {
        enemyMarkersController = enemyMarkersControllerObject.GetComponent<EnemyMarkersController>();
    }
}
