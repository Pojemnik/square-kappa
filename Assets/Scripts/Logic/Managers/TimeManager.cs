using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    [SerializeField]
    private TimeManagerConfig config;

    void Start()
    {
        if(config.slowTime <= 0 || config.slowTime >= 1)
        {
            throw new System.Exception("Slow time should be between 0 and 1");
        }
        EventManager.Instance.AddListener("SlowDownTime", SlowDownTime);
        EventManager.Instance.AddListener("ResetTimescale", ResetTimescale);
        EventManager.Instance.AddListener("StopTime", StopTime);
        EventManager.Instance.AddListener("PlayerDeath", delegate { TemporaryTimeStop(config.uiStopTimeDuration); });
        EventManager.Instance.AddListener("Victory", delegate { TemporaryTimeStop(config.uiStopTimeDuration); });
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
        Time.timeScale = config.slowTime;
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
