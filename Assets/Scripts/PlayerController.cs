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
    private Animator animator;
    private Vector3 lastMoveDelta;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        lastMoveDelta = Vector3.zero;
    }

    public void MoveXZ(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            return;
        }
        rawInputXZ = context.ReadValue<Vector2>();
        if (rawInputXZ.y > 0)
        {
            animator.SetTrigger("MoveForward");
        }
        else if (rawInputXZ.y < 0)
        {
            animator.SetTrigger("MoveBackward");
        }
    }

    public void MoveUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            return;
        }
        if (context.ReadValue<float>() == 1)
        {
            rawInputY = 1;
            animator.SetTrigger("MoveUpDown");
        }
        else
        {
            if (rawInputY > 0)
            {
                rawInputY = 0;
            }
        }
    }

    public void MoveDown(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            return;
        }
        if (context.ReadValue<float>() == 1)
        {
            rawInputY = -1;
            animator.SetTrigger("MoveUpDown");
        }
        else
        {
            if (rawInputY < 0)
            {
                rawInputY = 0;
            }
        }
    }

    private void Update()
    {
        Vector3 deltaSpeed = speed * Time.deltaTime;
        Vector3 moveDelta = new Vector3(rawInputXZ.x * deltaSpeed.x, rawInputY * deltaSpeed.y, rawInputXZ.y * deltaSpeed.z);
        if(moveDelta == Vector3.zero && lastMoveDelta != Vector3.zero)
        {
            animator.SetTrigger("Stop");
        }
        lastMoveDelta = moveDelta;
        rigidbody.AddForce(moveDelta);
    }
}
