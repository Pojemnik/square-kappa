using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacemarineMovement : UnitMovement
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

    public override Vector3 Velocity { get => rigidbody.velocity; }
    public override bool UseDrag
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

    public override bool IsRotating { get; protected set; }

    //input
    private Vector3 rawInput;
    private float rawInputRoll;

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
    private Vector2 rotationAnimationVector;

    private bool isGamePaused;

    public override void MoveXZ(Vector2 vector)
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

    public override void MoveY(float value)
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

    public override void Roll(float value)
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

    public override void RelativeLook(Vector2 deltaLook)
    {
        if (deltaLook != Vector2.zero)
        {
            Quaternion xRotation = Quaternion.AngleAxis(-deltaLook.x, Vector3.forward);
            Quaternion yRotation = Quaternion.AngleAxis(-deltaLook.y, Vector3.right);
            lookTarget = rigidbody.rotation * xRotation * yRotation;
            if (!isGamePaused)
            {
                rotationAnimationVector = Vector2.ClampMagnitude(deltaLook / 30, 0.3f);
                owner.AnimationController.SetRotationVector(rotationAnimationVector);
            }
        }
    }

    private Vector2 CalculateRoatationValue(Vector3 start, Vector3 target)
    {
        Vector3 rotationDelta = start - target;
        //Debug.LogFormat("Rotation delta: {0}", rotationDelta);
        Vector2 rotationValue = new Vector2(rotationDelta.y, rotationDelta.z) / 2;
        return rotationValue;
    }

    public override void MoveRelativeToCamera(Vector3 direction)
    {
        if (direction == Vector3.zero)
        {
            //owner.AnimationController.SetState("Stop");
        }
        else
        {
            direction = direction.normalized;
            owner.AnimationController.SetState("Move");
        }
        rawInput = direction;
    }

    public override void SetRotationImmediately(Vector3 direction)
    {
        lookTarget = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);
        shootDirection = direction;
    }

    public override void SetTargetRotation(Quaternion direction)
    {
        targetRotation = direction;
        startRotation = transform.rotation * Quaternion.Euler(-90, 0, 0);
        rotationStartTime = Time.time;
        rotationDuration = Quaternion.Angle(startRotation, targetRotation) / rotationSpeed;
        rotationValue = CalculateRoatationValue(startRotation * Vector3.forward, targetRotation * Vector3.forward);
        if (rotationDuration == 0)
        {
            IsRotating = false;
        }
        else
        {
            IsRotating = true;
        }
    }

    public override void MoveInGlobalCoordinates(Vector3 direction, bool patrolMode = true)
    {
        rigidbody.AddForce(Vector3.Scale(direction.normalized, speed) * Time.fixedDeltaTime);
        owner.AnimationController.SetState(patrolMode ? "PatrolMove" : "Move");
    }

    public override void MoveInGlobalCoordinatesIgnoringSpeed(Vector3 direction, bool patrolMode = true)
    {
        rigidbody.AddForce(direction * Time.fixedDeltaTime, ForceMode.VelocityChange);
        owner.AnimationController.SetState(patrolMode ? "PatrolMove" : "Move");
    }

    public override void MoveInGlobalCoordinatesIgnoringSpeedAndTimeDelta(Vector3 direction, bool patrolMode = true)
    {
        rigidbody.AddForce(direction, ForceMode.VelocityChange);
        owner.AnimationController.SetState(patrolMode ? "PatrolMove" : "Move");
    }

    public override void EnableStopMode()
    {
        owner.AnimationController.SetState("Stop");
    }

    private void MoveUnit()
    {
        Vector3 speedWithTime = speed * Time.fixedDeltaTime;
        Vector3 moveDelta = Vector3.Scale(rawInput, speedWithTime);
        Vector3[] cameraCooridinates = { firstPresonCamera.transform.right, firstPresonCamera.transform.up, firstPresonCamera.transform.forward };
        if (moveDelta == Vector3.zero)
        {
            if (lastMoveDelta != Vector3.zero)
            {
                //owner.AnimationController.SetState("Stop");
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
        if (rotationAnimationVector != Vector2.zero)
        {
            rotationAnimationVector = Vector2.MoveTowards(rotationAnimationVector, Vector2.zero, 0.03f);
            owner.AnimationController.SetRotationVector(rotationAnimationVector);
        }
    }

    private void CalculateSmoothRotation()
    {
        if (IsRotating == true)
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
                IsRotating = false;
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
            owner.TowardsTarget = Quaternion.LookRotation(firstPresonCamera.transform.forward, firstPresonCamera.transform.up);
        }
        else
        {
            if (shootDirection != Vector3.zero)
            {
                owner.TowardsTarget = Quaternion.LookRotation(shootDirection);
            }
        }
    }

    private void SetPause()
    {
        isGamePaused = true;
    }

    private void ResetPause()
    {
        isGamePaused = false;
    }

    protected override void Awake()
    {
        base.Awake();
        lastMoveDelta = Vector3.zero;
        lookTarget = rigidbody.rotation;
        rotationAnimationVector = Vector2.zero;
    }

    private void OnEnable()
    {
        EventManager.Instance?.AddListener("Pause", SetPause);
    }

    private void OnDisable()
    {
        EventManager.Instance?.AddListener("Unpause", ResetPause);
    }

    private void FixedUpdate()
    {
        MoveUnit();
        RotateUnit();
    }
}