using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryScreenController : MonoBehaviour
{
    private UIImageController imageController;

    void Start()
    {
        imageController = GetComponent<UIImageController>();
        EventManager eventManager = FindObjectOfType<EventManager>();
        eventManager.AddListener("Victory", imageController.DisplayScreen);
    }
}
