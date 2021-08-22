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
    [SerializeField]
    private float defaultDrag;

    [Header("Camera")]
    public bool cameraAiming;

    [Header("References")]
    public Unit owner;
    //[SerializeField]
    //private GameObject jetpack = null;
    [SerializeField]
    private GameObject firstPresonCamera;

    private Vector2 lastRotationValue;

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

    //input
    private Vector3 rawInput;
    private float rawInputRoll;

    //refernces
    private new Rigidbody rigidbody;

    //controllers
    //private JetpackController jetpackController;
    private PlayerCameraController cameraController;

    private Quaternion lookTarget;
    private Vector3 lastMoveDelta;
    private Vector3 shootDirection;

    private Quaternion targetRotation;
    private Quaternion startRotation;
    private float rotationStartTime;
    private float rotationDuration;
    private bool isRotating;
    public bool IsRotating { get => isRotating; }
    [SerializeField]
    private float rotationSpeed;

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

    private Vector2 CalculateRoatationValue(Vector3 lastRoatation)
    {
        Vector3 rotationDelta = lookTarget.eulerAngles - lastRoatation;
        if (rotationDelta.x > 180)
            rotationDelta.x -= 360;
        if (rotationDelta.x < -180)
            rotationDelta.x += 360;
        if (rotationDelta.z > 180)
            rotationDelta.z -= 360;
        if (rotationDelta.z < -180)
            rotationDelta.z += 360;
        Vector2 rotationValue = new Vector2(rotationDelta.z, rotationDelta.x);
        rotationValue.x = Mathf.Clamp(rotationValue.x / 5, -1, 1);
        rotationValue.y = Mathf.Clamp(rotationValue.y / 5, -1, 1);
        if (Mathf.Abs(rotationValue.x) > 0.5 || Mathf.Abs(rotationValue.y) > 0.5)
        {
            //Debug.LogFormat("Big rotation change: {0}", rotationValue);
            return Vector2.zero;
        }
        //Debug.LogFormat("Rotation change: {0}", rotationDelta);
        //Debug.LogFormat("Rotation value: {0}", rotationValue);
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
                shootDirection = direction * Vector3.forward;
            }
            else
            {
                isRotating = false;
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