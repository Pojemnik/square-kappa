using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//When new type of enemy is added this class needs to be made spacemarine-specific
//Then add base class/interface for it and any other movement type
public class UnitMovement : MonoBehaviour
{
    [Header("Movement parameters")]
    [SerializeField]
    private Vector3 speed;
    [SerializeField]
    private float rollSpeed;

    [Header("Camera")]
    public bool cameraAiming;

    [Header("References")]
    public Unit owner;
    [SerializeField]
    private GameObject jetpack = null;
    [SerializeField]
    private GameObject firstPresonCamera;

    public Vector3 Velocity { get => rigidbody.velocity; }

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

    public void MoveXZ(Vector2 vector)
    {
        rawInput.x = vector.x;
        rawInput.z = vector.y;
        if (rawInput.z > 0)
        {
            owner.AnimationController.SetState("MoveForward");
            jetpackController.OnMoveForward();
        }
        else if (rawInput.z < 0)
        {
            owner.AnimationController.SetState("MoveBackward");
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

    public void MoveY(float value)
    {
        if (value == 1)
        {
            rawInput.y = 1;
            owner.AnimationController.SetState("MoveUpDown");
            jetpackController.OnMoveUp();
        }
        else if (value == -1)
        {
            rawInput.y = -1;
            owner.AnimationController.SetState("MoveUpDown");
            jetpackController.OnMoveDown();
        }
        else
        {
            rawInput.y = 0;
        }
    }

    public void Roll(float value)
    {
        if (value == 1)
        {
            rawInputRoll = 1;
        }
        else if (value == -1)
        {
            rawInputRoll = -1;
        }
        else
        {
            rawInputRoll = 0;
        }
    }

    public void RelativeLook(Vector2 deltaLook)
    {
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

    public void MoveRelativeToCamera(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            direction = direction.normalized;
        }
        rawInput = direction;
    }

    public void MoveInGlobalCoordinates(Vector3 direction)
    {
        rigidbody.AddForce(Vector3.Scale(direction.normalized, speed) * Time.fixedDeltaTime);
    }

    private void MoveUnit()
    {
        Vector3 speedWithTime = speed * Time.fixedDeltaTime;
        Vector3 moveDelta = Vector3.Scale(rawInput, speedWithTime);
        Vector3[] cameraCooridinates = cameraController.orientation;
        if (moveDelta == Vector3.zero)
        {
            if (lastMoveDelta != Vector3.zero)
            {
                owner.AnimationController.SetState("Stop");
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