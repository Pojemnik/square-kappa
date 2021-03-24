using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Vector3 speed;

    private new Rigidbody rigidbody;
    private Vector2 rawInputXZ;
    private float rawInputY;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void MoveXZ(InputAction.CallbackContext context)
    {
        rawInputXZ = context.ReadValue<Vector2>();
    }

    public void MoveUp(InputAction.CallbackContext _)
    {
        rawInputY = 1;
    }

    public void MoveDown(InputAction.CallbackContext _)
    {
        rawInputY = -1;
    }

    private void Update()
    {
        Vector3 deltaSpeed = speed * Time.deltaTime;
        Vector3 moveDelta = new Vector3(rawInputXZ.x * deltaSpeed.x, rawInputY * deltaSpeed.y, rawInputXZ.y * deltaSpeed.z);
        rigidbody.AddForce(moveDelta);
    }
}
