using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public GameObject hitEffectPrefab;
    public int damage;
    [HideInInspector]
    public int[] ignoredLayers;
    [HideInInspector]
    public float speed;
}
