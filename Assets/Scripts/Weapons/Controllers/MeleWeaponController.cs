using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class MeleWeaponController : WeaponController
{
    public override WeaponConfig Config { get => meleConfig; }

    public override Quaternion AttackDirection { get => attackDirection; set { attackDirection = value; } }

    public override float Spread { get => 0; }

    public override MagazineStateType MagazineState => MagazineStateType.Full;

    [SerializeField]
    private MeleWeaponConfig meleConfig;

    private Quaternion attackDirection;
    private bool attacking;
    private CoroutineWrapper attackCoroutine;
    private AudioSource source;

    private GameObject GetParentWithHealth(Transform transform)
    {
        while (transform.parent != null && transform.gameObject.GetComponent<Health>() == null)
        {
            transform = transform.parent;
        }
        return transform.gameObject;
    }

    private IEnumerator AttackCoroutine(float timeBeforeHit, float timeAfterHit)
    {
        yield return new WaitForSeconds(timeBeforeHit);
        Vector3 startPos = attackDirection * -Vector3.forward * 0.3f + transform.position;
        if (Physics.SphereCast(startPos, 0.2f, attackDirection * Vector3.forward, out RaycastHit hit, meleConfig.range, KappaLayerMask.PlayerMeleAttackMask))
        {
            GameObject target = GetParentWithHealth(hit.transform);
            Health targetsHealth = target.GetComponent<Health>();
            if (targetsHealth != null)
            {
                targetsHealth.Damaged(new DamageInfo(meleConfig.damage, attackDirection * Vector3.forward, hit.point, hit.normal));
            }
            source.Play();
        }
        yield return new WaitForSeconds(timeAfterHit);
        InvokeAttackEvent();
    }

    public void OnAttackEnd()
    {
        if (attacking)
        {
            if (!attackCoroutine.Running)
            {
                attackCoroutine.Run(this);
            }
        }
    }

    private void Awake()
    {
        attacking = false;
        source = GetComponent<AudioSource>();
        attackCoroutine = new CoroutineWrapper(() => AttackCoroutine(meleConfig.damageTimeDelta, meleConfig.afterDamageTimeDelta));
        attackCoroutine.onCoroutineEnd += (s,e) => OnAttackEnd();
    }

    public override int Reload(int _) { return 1; }

    public override void SetTotalAmmo(int _) { }

    public override void StartAttack()
    {
        attacking = true;
        if (!attackCoroutine.Running)
        {
            attackCoroutine.Run(this);
        }
    }

    public override void StopAttack()
    {
        attacking = false;
    }
}
