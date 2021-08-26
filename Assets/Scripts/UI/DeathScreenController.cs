using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreenController : MonoBehaviour
{
    private UIImageController imageController;

    void Start()
    {
        imageController = GetComponent<UIImageController>();
        EventManager eventManager = FindObjectOfType<EventManager>();
        eventManager.AddListener("PlayerDeath", imageController.ShowScreen);
    }
}
