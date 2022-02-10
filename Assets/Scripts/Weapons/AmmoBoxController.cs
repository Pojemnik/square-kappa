using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBoxController : MonoBehaviour
{
    public SerializableDictionary<WeaponConfig.WeaponType, int> ammoCount;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlayerTrigger"))
        {
            if (audioSource != null)
            {
                audioSource.Play();
                Destroy(transform.parent.gameObject, audioSource.clip.length);
            }
            else
            {
                Destroy(transform.parent.gameObject);
            }
        }
    }
}
