using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [Header("References")]
    public GameObject hitEffectPrefab;
    [Header("Projectile's parameters")]
    public int damage;
    [SerializeField]
    protected float mass;
    [HideInInspector]
    public int[] ignoredLayers;
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public Vector3 direction;
}
