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

    public void OnPickup()
    {
        if (audioSource != null)
        {
            audioSource.Play();
            tag = "Untagged";
            Destroy(transform.parent.gameObject, audioSource.clip.length);
        }
        else
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
