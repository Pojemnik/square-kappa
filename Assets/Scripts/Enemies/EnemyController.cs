using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitShooting))]
[RequireComponent(typeof(UnitController))]
[RequireComponent(typeof(Health))]
public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public GameObject target;
    public UnitController unitController;
    public GameObject weapon;
    [SerializeField]
    private UnitShooting shooting;
    [SerializeField]
    private AI.StateMachine MovementStateMachine;
    [SerializeField]
    private AI.StateMachine ShootingStateMachine;

    [Header("Enemy properites")]
    public float VisionRange;
    public float targetDistance;
    [HideInInspector]
    public int layerMask;
    public AIShootingRules ShootingRules;
    public AIShootingRulesInterpretation ShootingRulesInterpretation;
    public float visibilitySphereRadius;
    public float visibilityConeAngle;
    public float visibilityConeHeight;
    public VisionGizmoCore gizmo;

    [Header("Ragdoll properities")]
    [SerializeField]
    private bool enableRagdoll;
    [SerializeField]
    [Tooltip("Maximum force applied to every rigidbody in a ragdoll on its start")]
    private float maxForce;

    private EnemyManager manager;

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
        DropWeapon();
        DeactivateComponents();
        manager.RemoveEnemy(gameObject);
        enabled = false;
    }

    private void DeactivateComponents()
    {
        MovementStateMachine.enabled = false;
        ShootingStateMachine.enabled = false;
        unitController.AnimationController.Deactivate();
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

    private void DrawGizmo(bool selected)
    {
        //VisionGizmo.DrawGizmo(selected);
    }

    private void Start()
    {
        manager = FindObjectOfType<EnemyManager>();
        manager.AddEnemy(gameObject);
        CrosshairController crosshair = FindObjectOfType<CrosshairController>();
        Health health = GetComponent<Health>();
        health.deathEvent.AddListener(crosshair.OnEnemyDeath);
        health.healthChangeEvent.AddListener(delegate { crosshair.OnEnemyHit(); });
        unitController.CurrentWeapon = weapon;
        unitController.AnimationController.UpdateWeaponAnimation(unitController.CurrentWeaponController);
        shooting.ChangeWeaponController(weapon.GetComponent<WeaponController>());
        unitController.movement.cameraAiming = false;
        layerMask = (1 << 7) | (1 << 8) | (1 << 9);
        layerMask = ~layerMask;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        DrawGizmo(true);
    }

    private void OnDrawGizmos()
    {
        DrawGizmo(false);
    }
#endif
}
