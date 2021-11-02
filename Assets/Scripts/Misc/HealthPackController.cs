using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPackController : MonoBehaviour
{
    [SerializeField]
    private int healAmount;

    public int HealAmount { get => healAmount; }

    public void Consume()
    {
        Destroy(transform.parent.gameObject);
    }
}
