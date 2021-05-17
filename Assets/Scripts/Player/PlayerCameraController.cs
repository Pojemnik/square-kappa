using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public GameObject player;

    private PlayerController playerController;

    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
    }

    void FixedUpdate()
    {
        int layerMask = (1 << 6) | (1 << 7); //do not include player and enemy layers
        layerMask = ~layerMask;
        Debug.DrawRay(transform.position, transform.forward, Color.magenta);
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, playerController.weaponPickupRange, layerMask))
        {
            playerController.SelectWorldItem(hit.collider.gameObject);
        }
        else
        {
            playerController.SelectWorldItem(null);
        }
    }
}
