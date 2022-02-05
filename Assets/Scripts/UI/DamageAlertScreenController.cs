using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAlertScreenController : MonoBehaviour
{
    private UIImageController imageController;
    private AudioSource audioSource;

    void Start()
    {
        imageController = GetComponent<UIImageController>();
        audioSource = GetComponent<AudioSource>();
        EventManager.Instance.AddListener("PlayerDamage", () => 
        {
            imageController.ShowScreenForTime();
            audioSource.Play(); 
        });
        EventManager.Instance.AddListener("PlayerDeath", imageController.HideScreen);
    }
}
