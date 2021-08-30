using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAlertScreenController : MonoBehaviour
{
    private UIImageController imageController;

    void Start()
    {
        imageController = GetComponent<UIImageController>();
        EventManager.Instance.AddListener("PlayerDamage", imageController.ShowScreenForTime);
        EventManager.Instance.AddListener("PlayerDeath", imageController.HideScreen);
    }
}
