using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField]
    private float slowTime;
    [SerializeField]
    private float uiStopTimeDuration;

    void Start()
    {
        if(slowTime <= 0 || slowTime >= 1)
        {
            throw new System.Exception("Slow time should be between 0 and 1");
        }
        EventManager eventManager = FindObjectOfType<EventManager>();
        eventManager.AddListener("SlowDownTime", SlowDownTime);
        eventManager.AddListener("ResetTimescale", ResetTimescale);
        eventManager.AddListener("StopTime", StopTime);
        eventManager.AddListener("PlayerDeath", delegate { TemporaryTimeStop(uiStopTimeDuration); });
        eventManager.AddListener("Victory", delegate { TemporaryTimeStop(uiStopTimeDuration); });
    }

    private void TemporaryTimeStop(float time)
    {
        StartCoroutine(TimeStopCoroutine(time));
    }

    private IEnumerator TimeStopCoroutine(float time)
    {
        StopTime();
        yield return new WaitForSecondsRealtime(time);
        ResetTimescale();
    }

    private void SlowDownTime()
    {
        Time.timeScale = slowTime;
    }

    private void ResetTimescale()
    {
        Time.timeScale = 1F;
    }

    private void StopTime()
    {
        Time.timeScale = 0;
    }
}
