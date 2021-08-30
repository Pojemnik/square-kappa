using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBoxController : MonoBehaviour
{
    public SerializableDictionary<WeaponConfig.WeaponType, int> ammoCount;

    private ItemsManager manager;

    private void Awake()
    {
        manager = FindObjectOfType<ItemsManager>();   
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
