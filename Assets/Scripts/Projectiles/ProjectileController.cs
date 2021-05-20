using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public GameObject hitEffectPrefab;
    public int damage;
    public int[] ignoredLayers;
    public float speed;
}
