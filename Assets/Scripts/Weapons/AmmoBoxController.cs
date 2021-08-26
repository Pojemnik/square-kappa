using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBoxController : MonoBehaviour
{
    public SerializableDictionary<WeaponConfig.WeaponType, int> ammoCount;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
