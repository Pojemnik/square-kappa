using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPackController : MonoBehaviour
{
    [SerializeField]
    private int healAmount;

    public int HealAmount { get => healAmount; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
