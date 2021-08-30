using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventsAdapter : MonoBehaviour
{
    [SerializeField]
    private bool callDeathEvent;

    private void Start()
    {
        Health health = GetComponent<Health>();
        health.deathEvent.AddListener(OnPlayerDeath);
        health.damageEvent.AddListener(delegate { OnPlayerDamage(); });
    }

    private void OnPlayerDeath()
    {
        if (callDeathEvent)
        {
            EventManager.Instance.TriggerEvent("PlayerDeath");
        }
    }

    private void OnPlayerDamage()
    {
        EventManager.Instance.TriggerEvent("PlayerDamage");
    }
}
