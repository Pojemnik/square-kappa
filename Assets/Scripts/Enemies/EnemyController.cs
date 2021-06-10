using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Refernces")]
    public GameObject target;
    public UnitController unitController;
    public GameObject weapon;

    [Header("Enemy properites")]
    public float ShootingRange;
    public float VisionRange;
    [HideInInspector]
    public int layerMask;
    public AIShootingRules ShootingRules;

    [SerializeField]
    private AIStateMachine AIStateMachine;

    public void OnDeath()
    {
        Destroy(gameObject);
    }

    void Start()
    {
        unitController.CurrentWeapon = weapon;
        unitController.movement.cameraAiming = false;
        layerMask = (1 << 7) | (1 << 8) | (1 << 9);
        layerMask = ~layerMask;
    }
}
