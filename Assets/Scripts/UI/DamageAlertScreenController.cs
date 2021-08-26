using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAlertScreenController : MonoBehaviour
{
    private UIImageController imageController;

    void Start()
    {
        imageController = GetComponent<UIImageController>();
        EventManager eventManager = FindObjectOfType<EventManager>();
        eventManager.AddListener("PlayerDamage", imageController.ShowScreen);
        eventManager.AddListener("PlayerDeath", imageController.HideScreen);
    }
}
