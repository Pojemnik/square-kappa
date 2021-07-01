using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitShooting))]
[RequireComponent(typeof(UnitController))]
[RequireComponent(typeof(Health))]
public class EnemyController : MonoBehaviour
{
    [Header("Refernces")]
    public GameObject target;
    public UnitController unitController;
    public GameObject weapon;
    [SerializeField]
    private UnitShooting shooting;
    [SerializeField]
    private AIStateMachine AIMovementStateMachine;
    [SerializeField]
    private AIStateMachine AIShootingStateMachine;

    [Header("Enemy properites")]
    public float VisionRange;
    public float targetDistance;
    [HideInInspector]
    public int layerMask;
    public AIShootingRules ShootingRules;
    public AIShootingRulesInterpretation ShootingRulesInterpretation;

    [Header("Ragdoll properities")]
    [SerializeField]
    private bool enableRagdoll;

    private EnemyManager manager;
    private Rigidbody[] childrenRigidbodies;

    public void OnDeath()
    {
        if (enableRagdoll)
        {
            StartRagdoll();
        }
        else
        {
            manager.RemoveEnemy(gameObject);
            Destroy(gameObject);
        }
    }

    private void StartRagdoll()
    {
        foreach (Rigidbody rb in childrenRigidbodies)
        {
            rb.isKinematic = false;
        }
        DropWeapon();
        DeactivateComponents();
        manager.RemoveEnemy(gameObject);
        enabled = false;
    }

    private void DeactivateComponents()
    {
        AIMovementStateMachine.enabled = false;
        AIShootingStateMachine.enabled = false;
        unitController.enabled = false;
        shooting.StopFire();
        shooting.enabled = false;
        GetComponent<Health>().enabled = false;
    }

    private void DropWeapon()
    {
        unitController.CurrentWeapon.transform.parent = null;
        unitController.CurrentWeapon.tag = "Item";
        unitController.CurrentWeapon.layer = 0;
        unitController.CurrentWeapon = null;
    }

    private void Start()
    {
        manager = FindObjectOfType<EnemyManager>();
        manager.AddEnemy(gameObject);
        unitController.CurrentWeapon = weapon;
        unitController.AnimationController.UpdateWeaponAnimation(unitController.CurrentWeaponController);
        shooting.ChangeWeaponController(weapon.GetComponent<WeaponController>());
        unitController.movement.cameraAiming = false;
        AIMovementStateMachine.ChangeState(new AIMoveTowardsTargetState());
        AIShootingStateMachine.ChangeState(new AIShootState());
        layerMask = (1 << 7) | (1 << 8) | (1 << 9);
        layerMask = ~layerMask;
        childrenRigidbodies = GetComponentsInChildren<Rigidbody>();
    }
}
