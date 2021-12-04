using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMovement : UnitMovement
{
    public override Vector3 Velocity { get; }

    public override bool UseDrag { get; set; }
    public override bool IsRotating { get; protected set; }

    [SerializeField]
    private Transform turretGun;
    [SerializeField]
    private Transform turretHead;
    [SerializeField]
    private float pitchSpeed;
    [SerializeField]
    private float yawSpeed;
    [SerializeField]
    private float pitchLimit;
    [SerializeField]
    private float yawLimit;

    private Quaternion turretHeadStartRotation;
    private Quaternion turretGunStartRotation;

    protected void Start()
    {
        turretGunStartRotation = turretGun.localRotation;
        turretHeadStartRotation = turretHead.localRotation;
    }

    public override void EnableStopMode()
    {
        throw new System.NotImplementedException();
    }

    public override void MoveInGlobalCoordinates(Vector3 direction, bool patrolMode = true)
    {
        throw new System.NotImplementedException();
    }

    public override void MoveInGlobalCoordinatesIgnoringSpeed(Vector3 direction, bool patrolMode = true)
    {
        throw new System.NotImplementedException();
    }

    public override void MoveInGlobalCoordinatesIgnoringSpeedAndTimeDelta(Vector3 direction, bool patrolMode = true)
    {
        throw new System.NotImplementedException();
    }

    public override void MoveRelativeToCamera(Vector3 direction)
    {
        throw new System.NotImplementedException();
    }

    public override void MoveXZ(Vector2 vector)
    {
        throw new System.NotImplementedException();
    }

    public override void MoveY(float value)
    {
        throw new System.NotImplementedException();
    }

    public override void RelativeLook(Vector2 deltaLook)
    {
        throw new System.NotImplementedException();
    }

    public override void Roll(float value)
    {
        throw new System.NotImplementedException();
    }

    public override void SetRotationImmediately(Vector3 direction)
    {
        Vector3 target = direction + transform.position;
        float angle;
        Vector3 targetRelative;
        Quaternion targetRotation;
        if (turretHead && (yawLimit != 0f))
        {
            targetRelative = turretHead.InverseTransformPoint(target);
            angle = Mathf.Atan2(targetRelative.x, targetRelative.z) * Mathf.Rad2Deg;
            if (angle >= 180f)
            {
                angle = 180f - angle;
            }
            if (angle <= -180f)
            {
                angle = -180f + angle;
            }
            targetRotation = turretHead.rotation * Quaternion.Euler(0f, Mathf.Clamp(angle, -yawSpeed * Time.deltaTime, yawSpeed * Time.deltaTime), 0f);
            if ((yawLimit < 360f) && (yawLimit > 0f))
            {
                turretHead.rotation = Quaternion.RotateTowards(turretHead.parent.rotation * turretHeadStartRotation, targetRotation, yawLimit);
            }
            else
            {
                turretHead.rotation = targetRotation;
            }
        }
        if (turretGun && (pitchLimit != 0f))
        {
            targetRelative = turretGun.InverseTransformPoint(target);
            angle = -Mathf.Atan2(targetRelative.y, targetRelative.z) * Mathf.Rad2Deg;
            if (angle >= 180f)
            {
                angle = 180f - angle;
            }
            if (angle <= -180f)
            {
                angle = -180f + angle;
            }
            targetRotation = turretGun.rotation * Quaternion.Euler(Mathf.Clamp(angle, -pitchSpeed * Time.deltaTime, pitchSpeed * Time.deltaTime), 0f, 0f);
            if ((pitchLimit < 360f) && (pitchLimit > 0f))
            {
                turretGun.rotation = Quaternion.RotateTowards(turretGun.parent.rotation * turretGunStartRotation, targetRotation, pitchLimit);
            }
            else
            {
                turretGun.rotation = targetRotation;
            }
        }
        owner.TowardsTarget = turretGun.rotation;
    }

    public override void SetTargetRotation(Quaternion direction)
    {
        SetTargetRotation(direction * Vector3.forward);
    }

    public override void SetTargetRotation(Vector3 direction)
    {
        SetRotationImmediately(direction);
    }
}
