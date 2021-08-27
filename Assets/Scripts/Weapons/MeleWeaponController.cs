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

    public override MagazineStateType MagazineState => MagazineStateType.Full;

    [SerializeField]
    private MeleWeaponConfig meleConfig;

    private UnityEvent attackEvent;
    private Quaternion attackDirection;
    private bool attacking;
    private float attackCooldown;
    private bool nextCollisionIsAttack;
    private Coroutine stopCoroutine;
    private GameObject currentCollision;
    private Vector3 contactPoint;
    private int mask;

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

    private void Attack()
    {
        nextCollisionIsAttack = true;
        AttackEvent.Invoke();
    }

    private GameObject GetTopParent(GameObject obj)
    {
        Transform transform = obj.transform;
        while (transform.parent != null)
        {
            transform = transform.parent;
        }
        return transform.gameObject;
    }
    private Vector3 GetPointOfContact(Vector3 target)
    {
        if (Physics.Raycast(transform.position, target - transform.position, out RaycastHit hit, float.PositiveInfinity, mask))
        {
            return hit.point;
        }
        Debug.LogError("Mele weapon hit error");
        return Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        currentCollision = other.gameObject;
        contactPoint = GetPointOfContact(currentCollision.transform.position);
    }

    private void OnTriggerExit(Collider other)
    {
        currentCollision = null;
    }

    private void Awake()
    {
        attackEvent = new UnityEvent();
        mask = ~(1 << gameObject.layer);
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
            GameObject topParent = GetTopParent(currentCollision);
            Health targetsHealth = topParent.GetComponent<Health>();
            if (targetsHealth != null)
            {
                //Change direction when mele ememies are added
                Vector3 normal = (transform.position - currentCollision.transform.position).normalized;
                targetsHealth.Damaged(new DamageInfo(meleConfig.damage, transform.forward, contactPoint, normal));
            }
            nextCollisionIsAttack = false;
        }
    }

    private IEnumerator StopAttackCoroutine()
    {
        yield return new WaitForSeconds(1F / meleConfig.attacksPerSecond);
        nextCollisionIsAttack = false;
    }

    public override int Reload(int _) { return 1; }

    public override void SetTotalAmmo(int _) { }
}
