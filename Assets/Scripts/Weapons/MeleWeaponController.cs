using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class MeleWeaponController : WeaponController
{
    public override WeaponConfig Config { get => meleConfig; }

    public override UnityEvent AttackEvent { get => attackEvent; }

    public override Quaternion AttackDirection { get => attackDirection; set { attackDirection = value; } }

    public override float Spread { get => 0; }

    public override void StartAttack()
    {
        attacking = true;
        if (stopCoroutine != null)
        {
            StopCoroutine(stopCoroutine);
        }
    }

    public override void StopAttack()
    {
        attacking = false;
        stopCoroutine = StartCoroutine(StopAttackCoroutine());
    }

    [SerializeField]
    private MeleWeaponConfig meleConfig;
    private UnityEvent attackEvent;
    private Quaternion attackDirection;
    private bool attacking;
    private float attackCooldown;
    private bool nextCollisionIsAttack;
    private Coroutine stopCoroutine;
    private GameObject currentCollision;

    private void Attack()
    {
        nextCollisionIsAttack = true;
        AttackEvent.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        currentCollision = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        currentCollision = null;
    }

    private void Awake()
    {
        attackEvent = new UnityEvent();
    }

    private void FixedUpdate()
    {
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.fixedDeltaTime;
        }
        if (attacking && attackCooldown <= 0)
        {
            Attack();
            attackCooldown = 1F / meleConfig.attacksPerSecond;
        }
        if (nextCollisionIsAttack && currentCollision != null)
        {
            print(currentCollision);
            nextCollisionIsAttack = false;
        }
    }

    private IEnumerator StopAttackCoroutine()
    {
        yield return new WaitForSeconds(1F / meleConfig.attacksPerSecond);
        nextCollisionIsAttack = false;
    }
}
