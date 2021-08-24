using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitShooting))]
[RequireComponent(typeof(UnitController))]
[RequireComponent(typeof(Health))]
public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [HideInInspector]
    public GameObject target;
    public UnitController unitController;
    public GameObject weapon;
    [SerializeField]
    private UnitShooting shooting;
    public GameObject head;

    [Header("Enemy properites")]
    public AIShootingRules ShootingRules;
    public AIShootingRulesInterpretation ShootingRulesInterpretation;
    public float visibilitySphereRadius;
    public float visibilityConeAngle;
    public float visibilityConeHeight;
    [ReadOnly]
    [SerializeField]
    private float visibilityConeRadius;

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
    private GameObject ragdollPrefab;

    private EnemyManager manager;
    private VisionGizmoCore gizmo;

    public void OnDeath()
    {
        if (enableRagdoll)
        {
            StartRagdoll();
        }
        manager.RemoveEnemy(gameObject);
        Destroy(gameObject);
    }

    private void StartRagdoll()
    {
        DropWeapon();
        Instantiate(ragdollPrefab, transform.position, transform.rotation);
        DeactivateComponents();
    }

    private void DeactivateComponents()
    {
        unitController.AnimationController.Deactivate();
        unitController.enabled = false;
        shooting.StopFire();
        shooting.enabled = false;
        GetComponent<Health>().enabled = false;
        GetComponent<AI.StateMachine>().enabled = false;
    }

    private void DropWeapon()
    {
        unitController.CurrentWeapon.transform.parent = null;
        unitController.CurrentWeapon.tag = "Item";
        unitController.CurrentWeapon.layer = 14;
        unitController.CurrentWeapon = null;
    }

    private void DrawGizmo(bool selected)
    {
        gizmo.Draw(selected, head.transform);
    }

    private void Start()
    {
        manager = FindObjectOfType<EnemyManager>();
        target = manager.target;
        manager.AddEnemy(gameObject);
        CrosshairController crosshair = FindObjectOfType<CrosshairController>();
        Health health = GetComponent<Health>();
        health.deathEvent.AddListener(crosshair.OnEnemyDeath);
        health.healthChangeEvent.AddListener(delegate { crosshair.OnEnemyHit(); });
        unitController.CurrentWeapon = weapon;
        unitController.AnimationController.UpdateWeaponAnimation(unitController.CurrentWeaponController);
        shooting.ChangeWeaponController(weapon.GetComponent<WeaponController>());
        shooting.IgnoreRecoil = true;
        unitController.movement.cameraAiming = false;
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
        visibilityConeRadius = gizmo.ConeBaseRadius;
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
