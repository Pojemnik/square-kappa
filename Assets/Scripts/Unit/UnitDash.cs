using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDash : MonoBehaviour
{
    [SerializeField]
    private float dashForceMultipler;
    [SerializeField]
    private float dashStopForceMultipler;
    [SerializeField]
    private float dashCooldownTime;

    private bool canDash;
    private bool dashMode;
    private new Rigidbody rigidbody;

    public void EnableDashMode()
    {
        if (canDash)
        {
            dashMode = true;
            EventManager.Instance.TriggerEvent("SlowDownTime");
        }
    }

    public void DisableDashMode()
    {
        dashMode = false;
        EventManager.Instance.TriggerEvent("ResetTimescale");
    }

    private IEnumerator DashCoroutine(Vector3 force)
    {
        EventManager.Instance.TriggerEvent("ResetTimescale");
        rigidbody.AddForce(force, ForceMode.VelocityChange);
        yield return new WaitForSecondsRealtime(1);
        rigidbody.AddForce(-force * dashStopForceMultipler, ForceMode.VelocityChange);
    }
    
    private IEnumerator DashCooldownCoroutine(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        canDash = true;
        print("Can dash now");
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        canDash = true;
    }

    private void FixedUpdate()
    {
        if(dashMode)
        {
            dashMode = false;
            canDash = false;
            //TODO: Determining direction of dash - needs new player controls adapter
            //StartCoroutine(DashCoroutine(dashForceMultipler * rigidbody.velocity.normalized));
            //StartCoroutine(DashCooldownCoroutine(dashCooldownTime));
        }
    }
}
