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
    private float defaultDrag;

    [Header("Rotation parameters")]
    [SerializeField]
    private float rollSpeed;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float rotationAnimationSmoothness;
    [SerializeField]
    private float rotationAnimationMaxValue;

    [Header("Camera")]
    public bool cameraAiming;

    [Header("References")]
    public Unit owner;
    [SerializeField]
    private GameObject firstPresonCamera;

    public Vector3 Velocity { get => rigidbody.velocity; }
    public bool UseDrag
    {
        get => rigidbody.drag == 0;
        set
        {
            if (value)
            {
                rigidbody.drag = defaultDrag;
            }
            else
            {
                rigidbody.drag = 0;
            }
        }
    }

    public bool IsRotating { get => isRotating; }
    private bool isRotating;

    //input
    private Vector3 rawInput;
    private float rawInputRoll;

    //refernces
    private new Rigidbody rigidbody;

    //controllers
    private PlayerCameraController cameraController;

    //movement
    private Vector3 lastMoveDelta;

    //rotations
    private Vector3 shootDirection;
    private Quaternion lookTarget;
    private Quaternion targetRotation;
    private Quaternion startRotation;
    private float rotationStartTime;
    private float rotationDuration;
    private Vector2 rotationValue;

    public void MoveXZ(Vector2 vector)
    {
        rawInput.x = vector.x;
        rawInput.z = vector.y;
        if (rawInput.z > 0)
        {
            owner.AnimationController.SetState("Move");
        }
        else if (rawInput.z < 0)
        {
            owner.AnimationController.SetState("Move");
        }
        if (rawInput.x > 0)
        {
            owner.AnimationController.SetState("Move");
        }
        else if (rawInput.x < 0)
        {
            owner.AnimationController.SetState("Move");
        }
    }

    public void MoveY(float value)
    {
        if (value == 1)
        {
            rawInput.y = 1;
            owner.AnimationController.SetState("Move");
        }
        else if (value == -1)
        {
            rawInput.y = -1;
            owner.AnimationController.SetState("Move");
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

    private Vector2 CalculateRoatationValue(Vector3 start, Vector3 target)
    {
        Vector3 rotationDelta = start - target;
        //Debug.LogFormat("Rotation delta: {0}", rotationDelta);
        Vector2 rotationValue = new Vector2(rotationDelta.y, rotationDelta.z) / 2;
        return rotationValue;
    }

    public void MoveRelativeToCamera(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            direction = direction.normalized;
            owner.AnimationController.SetState("Move");
        }
        rawInput = direction;
    }

    public void SetRotationImmediately(Vector3 direction)
    {
        lookTarget = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);
        shootDirection = direction;
    }

    public void SetTargetRotation(Vector3 direction)
    {
        SetTargetRotation(Quaternion.LookRotation(direction));
    }

    public void SetTargetRotation(Quaternion direction)
    {
        targetRotation = direction;
        startRotation = transform.rotation * Quaternion.Euler(-90, 0, 0);
        rotationStartTime = Time.time;
        rotationDuration = Quaternion.Angle(startRotation, targetRotation) / rotationSpeed;
        rotationValue = CalculateRoatationValue(startRotation * Vector3.forward, targetRotation * Vector3.forward);
        if (rotationDuration == 0)
        {
            isRotating = false;
        }
        else
        {
            isRotating = true;
        }
    }

    public void MoveInGlobalCoordinates(Vector3 direction)
    {
        rigidbody.AddForce(Vector3.Scale(direction.normalized, speed) * Time.fixedDeltaTime);
        owner.AnimationController.SetState("Move");
    }

    public void MoveInGlobalCoordinatesIgnoringSpeed(Vector3 direction)
    {
        rigidbody.AddForce(direction * Time.fixedDeltaTime, ForceMode.VelocityChange);
        owner.AnimationController.SetState("Move");
    }

    public void MoveInGlobalCoordinatesIgnoringSpeedAndTimeDelta(Vector3 direction)
    {
        rigidbody.AddForce(direction, ForceMode.VelocityChange);
        owner.AnimationController.SetState("Move");
    }

    public void EnableStopMode()
    {
        owner.AnimationController.SetState("Stop");
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
        CalculateSmoothRotation();
        lookTarget = rigidbody.rotation;
        SetAim();
    }

    private void CalculateSmoothRotation()
    {
        if (isRotating == true)
        {
            float t = (Time.time - rotationStartTime) / rotationDuration;
            if (t < 1)
            {
                Quaternion direction = Quaternion.Slerp(startRotation, targetRotation, t) * Quaternion.Euler(90, 0, 0);
                rigidbody.MoveRotation(direction);
                SetRotationAnimation(t);
                shootDirection = direction * Vector3.forward;
            }
            else
            {
                isRotating = false;
                owner.AnimationController.ResetStaticState("Rotation");
                owner.AnimationController.SetRotationVector(Vector2.zero);
            }
        }
    }

    private void SetRotationAnimation(float t)
    {
        owner.AnimationController.SetStaticState("Rotation");
        float normalisationParamaeter = 1 / rotationAnimationSmoothness;
        if (t < rotationAnimationSmoothness)
        {
            owner.AnimationController.SetRotationVector(rotationValue * normalisationParamaeter * t * rotationAnimationMaxValue);
        }
        else
        {
            if (t < 1 - rotationAnimationSmoothness)
            {
                owner.AnimationController.SetRotationVector(rotationValue * rotationAnimationMaxValue);
            }
            else
            {
                owner.AnimationController.SetRotationVector(rotationValue * normalisationParamaeter * (1 - t) * rotationAnimationMaxValue);
            }
        }
    }

    private void SetAim()
    {
        if (cameraAiming)
        {
            owner.TowardsTarget = Quaternion.LookRotation(cameraController.orientation[2], cameraController.orientation[1]);
        }
        else
        {
            if (shootDirection != Vector3.zero)
            {
                owner.TowardsTarget = Quaternion.LookRotation(shootDirection);
            }
        }
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
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