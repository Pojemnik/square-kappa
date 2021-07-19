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

    [Header("Gizmo properties")]
    [SerializeField]
    [Min(1)]
    private int points;
    [SerializeField]
    [Min(1)]
    private int coneLines;
    [SerializeField]
    private bool drawWhenSelectedOnly;
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Color notSelectedColor;

    [Header("Ragdoll properities")]
    [SerializeField]
    private bool enableRagdoll;
    [SerializeField]
    [Tooltip("Maximum force applied to every rigidbody in a ragdoll on its start")]
    private float maxForce;

    private EnemyManager manager;
    private VisionGizmoCore gizmo;

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
        gizmo.Draw(selected, transform);
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
        shooting.ignoreRecoil = true;
        unitController.movement.cameraAiming = false;
        layerMask = (1 << 7) | (1 << 8) | (1 << 9);
        layerMask = ~layerMask;
#if UNITY_EDITOR
        gizmo = new VisionGizmoCore();
        UpdateGizmoProperties();
#endif
    }

#if UNITY_EDITOR
    private void UpdateGizmoProperties()
    {
        gizmo.ConeAngle = visibilityConeAngle;
        gizmo.ConeHeight = visibilityConeHeight;
        gizmo.SphereRadius = visibilitySphereRadius;
        gizmo.Points = points;
        gizmo.NotSelectedColor = notSelectedColor;
        gizmo.SelectedColor = selectedColor;
        gizmo.ConeLines = coneLines;
        gizmo.DrawWhenSelectedOnly = drawWhenSelectedOnly;
    }

    private void OnValidate()
    {
        if (gizmo == null)
        {
            gizmo = new VisionGizmoCore();
        }
        UpdateGizmoProperties();
    }

    private void OnDrawGizmosSelected()
    {
        DrawGizmo(true);
    }

    private void OnDrawGizmos()
    {
        DrawGizmo(false);
    }
#endif
}
