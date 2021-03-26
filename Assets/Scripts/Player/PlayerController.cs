using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Vector3 speed;
    public float rollSpeed;
    public GameObject jetpack = null;
    public float cameraSensitivity;

    private new Rigidbody rigidbody;
    private Vector2 rawInputXZ;
    private float rawInputY;
    private float rawInputRoll;
    private Vector2 rawInputLook;
    private Animator animator;
    private Vector3 lastMoveDelta;
    private JetpackController jetpackController;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        lastMoveDelta = Vector3.zero;
        jetpackController = jetpack.GetComponent<JetpackController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
            jetpackController.OnMoveForward();
        }
        else if (rawInputXZ.y < 0)
        {
            animator.SetTrigger("MoveBackward");
            jetpackController.OnMoveBackward();
        }
        if (rawInputXZ.x > 0)
        {
            jetpackController.OnMoveRight();
        }
        else if (rawInputXZ.x < 0)
        {
            jetpackController.OnMoveLeft();
        }
    }

    public void MoveVertical(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            return;
        }
        if (context.ReadValue<float>() == 1)
        {
            rawInputY = 1;
            animator.SetTrigger("MoveUpDown");
            jetpackController.OnMoveUp();
        }
        else if (context.ReadValue<float>() == -1)
        {
            rawInputY = -1;
            animator.SetTrigger("MoveUpDown");
            jetpackController.OnMoveDown();
        }
        else
        {
            rawInputY = 0;
        }
    }

    public void Roll(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            return;
        }
        if (context.ReadValue<float>() == 1)
        {
            rawInputRoll = 1;
        }
        else if (context.ReadValue<float>() == -1)
        {
            rawInputRoll = -1;
        }
        else
        {
            rawInputRoll = 0;
        }
    }

    public void Look(InputAction.CallbackContext context)
    {
        rawInputLook = context.ReadValue<Vector2>();
        rawInputLook = new Vector2(-rawInputLook.y, rawInputLook.x);
    }

    private void MovePlayer()
    {
        Vector3 deltaSpeed = speed * Time.fixedDeltaTime;
        Vector3 moveDelta = new Vector3(rawInputXZ.x * deltaSpeed.x, rawInputY * deltaSpeed.y, rawInputXZ.y * deltaSpeed.z);
        if (moveDelta == Vector3.zero && lastMoveDelta != Vector3.zero)
        {
            animator.SetTrigger("Stop");
            jetpackController.OnStop();
        }
        lastMoveDelta = moveDelta;
        rigidbody.AddRelativeForce(moveDelta);
    }

    private void RotatePlayer()
    {
        float deltaRoll = rollSpeed * Time.fixedDeltaTime * rawInputRoll;
        Vector2 deltaLook = rawInputLook * cameraSensitivity;
        rigidbody.AddRelativeTorque(0, 0, deltaRoll);
        Quaternion targetRotation = Quaternion.Euler((Vector3)deltaLook) * rigidbody.rotation;
        rigidbody.MoveRotation(targetRotation);
    }

    private void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
    }
}
