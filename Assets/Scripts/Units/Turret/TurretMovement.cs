using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMovement : UnitMovement
{
    public override Vector3 Velocity { get; }

    public override bool UseDrag { get; set; }
    public override bool IsRotating { get; protected set; }

    //[SerializeField]
    //private Transform turretHead;
    //[SerializeField]
    //private Transform turretGun;
    [SerializeField]
    private Turret turret;

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
        turret.target = direction + transform.position;
        Debug.Log(direction);
        //Vector3 d = transform.InverseTransformDirection(direction);
        //Vector3 xRotDir = transform.TransformDirection(new Vector3(0, d.y, 0));
        //Vector3 yRotDir = transform.TransformDirection(new Vector3(d.x, 0, 0));
        //Quaternion xRot = Quaternion.LookRotation(xRotDir);
        //Quaternion yRot = Quaternion.LookRotation(yRotDir);
        //turretGun.rotation = xRot;
        //turretHead.rotation = yRot;
        //Debug.Log("XD");
        //if (cameraAiming)
        //{
        //    owner.TowardsTarget = Quaternion.LookRotation(firstPresonCamera.transform.forward, firstPresonCamera.transform.up);
        //}
        //else
        //{
        //    if (direction != Vector3.zero)
        //    {
        //        owner.TowardsTarget = Quaternion.LookRotation(direction);
        //    }
        //}
    }

    public override void SetTargetRotation(Quaternion direction)
    {
        //throw new System.NotImplementedException();
    }
}
