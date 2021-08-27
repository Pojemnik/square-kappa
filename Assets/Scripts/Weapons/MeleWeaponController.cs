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

    [SerializeField]
    private MeleWeaponConfig meleConfig;

    private UnityEvent attackEvent;
    private Quaternion attackDirection;
    /*
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
*/

    private void Update()
    {
        Vector3 startPos = attackDirection * -Vector3.forward + transform.position;
        Debug.DrawLine(startPos, transform.position + attackDirection * Vector3.forward * meleConfig.range, Color.green);
    }

    private void Awake()
    {
        attackEvent = new UnityEvent();
    }

    public void OnAttackEnd()
    {
        attackEvent.Invoke();
        Vector3 startPos = attackDirection * -Vector3.forward + transform.position;
        if (Physics.SphereCast(startPos, 0.5f, attackDirection * Vector3.forward, out RaycastHit hit, meleConfig.range, KappaLayerMask.PlayerMeleAttackMask))
        {
            Debug.LogFormat("Target hit: {0}", hit.collider.gameObject.name);
        }
    }

    public override int Reload(int _) { return 1; }

    public override void SetTotalAmmo(int _) { }

    public override void StartAttack() { }

    public override void StopAttack() { }
}
