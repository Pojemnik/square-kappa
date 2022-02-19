using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitMovement : MonoBehaviour
{
    [Header("Camera")]
    public bool cameraAiming;

    [Header("References")]
    public Unit owner;

    protected new Rigidbody rigidbody;

    [SerializeField]
    protected GameObject firstPresonCamera;

    public abstract Vector3 Velocity { get; }
    public abstract bool UseDrag
    {
        get; set;
    }
    public abstract bool IsRotating { get; protected set; }

    protected virtual void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public abstract void MoveXZ(Vector2 vector);
    public abstract void MoveY(float value);
    public abstract void Roll(float value);
    public abstract void RelativeLook(Vector2 deltaLook);
    public abstract void MoveRelativeToCamera(Vector3 direction);
    public abstract void SetRotationImmediately(Vector3 direction);
    public virtual void SetTargetRotation(Vector3 direction)
    {
        SetTargetRotation(Quaternion.LookRotation(direction));
    }
    public abstract void SetTargetRotation(Quaternion direction);
    public abstract void MoveInGlobalCoordinates(Vector3 direction, bool patrolMode = true);
    public abstract void MoveInGlobalCoordinatesIgnoringSpeed(Vector3 direction, bool patrolMode = true);
    public abstract void MoveInGlobalCoordinatesIgnoringSpeedAndTimeDelta(Vector3 direction, bool patrolMode = true);
    public abstract void EnableStopMode();
    public virtual void SlowDown()
    {
        MoveInGlobalCoordinates(-Velocity, false);
    }
}
