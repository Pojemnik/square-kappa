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

    public override void StartAttack()
    {
        attacking = true;
    }

    public override void StopAttack()
    {
        attacking = false;
    }

    [SerializeField]
    private MeleWeaponConfig meleConfig;
    private UnityEvent attackEvent;
    private Quaternion attackDirection;
    private bool attacking;
    private float attackCooldown;

    private void Attack()
    {
        AttackEvent.Invoke();
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
    }
}
