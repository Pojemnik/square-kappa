using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventsAdapter : MonoBehaviour
{
    [SerializeField]
    private bool callDeathEvent;

    private Health health;

    private void Start()
    {
        health = GetComponent<Health>();
        health.deathEvent.AddListener(OnPlayerDeath);
        health.damageEvent.AddListener(delegate { OnPlayerDamage(); });
        EventManager.Instance.AddListener("ReloadScene", OnGameReolad);
    }

    private void OnGameReolad()
    {
        health.Heal();
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
