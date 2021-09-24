using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MeleWeaponController : WeaponController
{
    public override WeaponConfig Config { get => meleConfig; }

    public override UnityEvent AttackEvent { get => attackEvent; }

    public override Quaternion AttackDirection { get => attackDirection; set { attackDirection = value; } }

    public override float Spread { get => 0; }

    public override MagazineStateType MagazineState => MagazineStateType.Full;

    public override UnityEvent<(int, int)> AmmoChangeEvent => new UnityEvent<(int, int)>();

    [SerializeField]
    private MeleWeaponConfig meleConfig;

    private UnityEvent attackEvent;
    private Quaternion attackDirection;
    private bool attacking;
    private bool coroutineRunning;

    private GameObject GetParentWithHealth(Transform transform)
    {
        while (transform.parent != null && transform.gameObject.GetComponent<Health>() == null)
        {
            transform = transform.parent;
        }
        return transform.gameObject;
    }

    private IEnumerator AttackCoroutine(float timeDelta)
    {
        coroutineRunning = true;
        yield return new WaitForSeconds(timeDelta);
        attackEvent.Invoke();
        Vector3 startPos = attackDirection * -Vector3.forward * 0.3f + transform.position;
        if (Physics.SphereCast(startPos, 0.2f, attackDirection * Vector3.forward, out RaycastHit hit, meleConfig.range, KappaLayerMask.PlayerMeleAttackMask))
        {
            //Debug.LogFormat("Target hit: {0}", hit.collider.gameObject.name);
            GameObject target = GetParentWithHealth(hit.transform);
            Health targetsHealth = target.GetComponent<Health>();
            if (targetsHealth != null)
            {
                targetsHealth.Damaged(new DamageInfo(meleConfig.damage, attackDirection * Vector3.forward, hit.point, hit.normal));
            }
        }
        coroutineRunning = false;
    }

    public void OnAttackEnd()
    {
        if (attacking)
        {
            if (!coroutineRunning)
            {
                StartCoroutine(AttackCoroutine(meleConfig.damageTimeDelta));
            }
        }
    }

    private void Awake()
    {
        attackEvent = new UnityEvent();
        attacking = false;
    }

    public override int Reload(int _) { return 1; }

    public override void SetTotalAmmo(int _) { }

    public override void StartAttack()
    {
        attacking = true;
        StartCoroutine(AttackCoroutine(meleConfig.damageTimeDelta));
    }

    public override void StopAttack()
    {
        attacking = false;
    }
}
