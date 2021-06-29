using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitShooting))]
[RequireComponent(typeof(UnitController))]
public class EnemyController : MonoBehaviour
{
    [Header("Refernces")]
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

    [SerializeField]
    private AIStateMachine AIMovementStateMachine;
    [SerializeField]
    private AIStateMachine AIShootingStateMachine;

    private EnemyManager manager;

    public void OnDeath()
    {
        manager.RemoveEnemy(gameObject);
        Destroy(gameObject);
    }

    void Start()
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
    }
}
