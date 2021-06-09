using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitMovement : MonoBehaviour
{
    [Header("Movement parameters")]
    [SerializeField]
    private Vector3 speed;
    [SerializeField]
    private float rollSpeed;

    [Header("Camera")]
    [SerializeField]
    private float cameraSensitivity;

    [Header("References")]
    public Unit owner;
    [SerializeField]
    private GameObject jetpack = null;
    [SerializeField]
    private GameObject firstPresonCamera;
    [SerializeField]
    private bool cameraAiming;

    //input
    private Vector3 rawInput;
    private float rawInputRoll;

    //refernces
    private new Rigidbody rigidbody;

    //controllers
    private JetpackController jetpackController;
    private PlayerCameraController cameraController;

    private Quaternion lookTarget;
    private Vector3 lastMoveDelta;
    private Vector3 shootDirection;

    public void MoveXZ(InputAction.CallbackContext context)
    {
        Vector2 rawInputXZ = context.ReadValue<Vector2>();
        rawInput.x = rawInputXZ.x;
        rawInput.z = rawInputXZ.y;
        if (rawInput.z > 0)
        {
            owner.UnitAnimator.SetTrigger("MoveForward");
            jetpackController.OnMoveForward();
        }
        else if (rawInput.z < 0)
        {
            owner.UnitAnimator.SetTrigger("MoveBackward");
            jetpackController.OnMoveBackward();
        }
        if (rawInput.x > 0)
        {
            jetpackController.OnMoveRight();
        }
        else if (rawInput.x < 0)
        {
            jetpackController.OnMoveLeft();
        }
    }

    public void MoveY(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() == 1)
        {
            rawInput.y = 1;
            owner.UnitAnimator.SetTrigger("MoveUpDown");
            jetpackController.OnMoveUp();
        }
        else if (context.ReadValue<float>() == -1)
        {
            rawInput.y = -1;
            owner.UnitAnimator.SetTrigger("MoveUpDown");
            jetpackController.OnMoveDown();
        }
        else
        {
            rawInput.y = 0;
        }
    }

    public void Roll(InputAction.CallbackContext context)
    {
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
    public void RelativeLook(InputAction.CallbackContext context)
    {
        Vector2 rawInputLook = context.ReadValue<Vector2>();
        Vector2 deltaLook = rawInputLook * cameraSensitivity;
        if (deltaLook != Vector2.zero)
        {
            Quaternion xRotation = Quaternion.AngleAxis(-deltaLook.x, Vector3.forward);
            Quaternion yRotation = Quaternion.AngleAxis(-deltaLook.y, Vector3.right);
            lookTarget = rigidbody.rotation * xRotation * yRotation;
        }
    }

    public void SetLookTarget(Vector3 direction)
    {
        lookTarget = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);
        shootDirection = direction;
    }

    public void MoveTowards(Vector3 direction)
    {
        direction = direction.normalized;
        direction = transform.right * direction.x + transform.up * direction.y + transform.forward * direction.z;
        rawInput = direction;
        rawInput.z = -rawInput.z;
    }

    public void MoveRelative(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            direction = direction.normalized;
        }
        rawInput = direction;
    }

    private void MoveUnit()
    {
        Vector3 speedWithTime = speed * Time.fixedDeltaTime;
        Vector3 moveDelta = new Vector3(rawInput.x * speedWithTime.x, rawInput.y * speedWithTime.y, rawInput.z * speedWithTime.z);
        Vector3[] cameraCooridinates = cameraController.orientation;
        if (moveDelta == Vector3.zero)
        {
            if (lastMoveDelta != Vector3.zero)
            {
                owner.UnitAnimator.SetTrigger("Stop");
                jetpackController.OnStop();
            }
        }
        else
        {
            Vector3 globalDelta = LocalToGlobalMovement(moveDelta, cameraCooridinates);
            rigidbody.AddForce(globalDelta);
        }
        lastMoveDelta = moveDelta;
    }

    private Vector3 LocalToGlobalMovement(Vector3 moveDelta, Vector3[] cameraCooridinates)
    {
        Vector3 globalDelta = cameraCooridinates[0] * moveDelta.x + cameraCooridinates[1] * moveDelta.y + cameraCooridinates[2] * moveDelta.z;
        return globalDelta;
    }

    private void RotateUnit()
    {
        float deltaRoll = rollSpeed * Time.fixedDeltaTime * rawInputRoll;
        rigidbody.MoveRotation(lookTarget * Quaternion.Euler(0, deltaRoll, 0));
        lookTarget = rigidbody.rotation;
        if (cameraAiming)
        {
            owner.TowardsTarget = Quaternion.LookRotation(cameraController.orientation[2], cameraController.orientation[1]);
        }
        else
        {
            owner.TowardsTarget = Quaternion.LookRotation(shootDirection);
        }
    }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        if (jetpack != null)
        {
            jetpackController = jetpack.GetComponent<JetpackController>();
        }
        cameraController = firstPresonCamera.GetComponent<PlayerCameraController>();
        lastMoveDelta = Vector3.zero;
        lookTarget = rigidbody.rotation;
    }

    private void FixedUpdate()
    {
        MoveUnit();
        RotateUnit();
    }
}