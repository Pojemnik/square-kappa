using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField]
    private float slowTime;

    void Start()
    {
        if(slowTime <= 0 || slowTime >= 1)
        {
            throw new System.Exception("Slow time should be between 0 and 1");
        }
        EventManager eventManager = FindObjectOfType<EventManager>();
        eventManager.AddListener("SlowDownTime", SlowDownTime);
        eventManager.AddListener("ResetTimescale", ResetTimescale);
    }

    private void SlowDownTime()
    {
        Time.timeScale = slowTime;
    }

    private void ResetTimescale()
    {
        Time.timeScale = 1F;
    }
}
