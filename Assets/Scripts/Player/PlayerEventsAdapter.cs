using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventsAdapter : MonoBehaviour
{
    [SerializeField]
    private bool callDeathEvent;

    private EventManager eventManager;

    private void Start()
    {
        Health health = GetComponent<Health>();
        health.deathEvent.AddListener(OnPlayerDeath);
        health.damageEvent.AddListener(delegate { OnPlayerDamage(); });
        eventManager = FindObjectOfType<EventManager>();
    }

    private void OnPlayerDeath()
    {
        if (callDeathEvent)
        {
            eventManager.TriggerEvent("PlayerDeath");
        }
    }

    private void OnPlayerDamage()
    {
        eventManager.TriggerEvent("PlayerDamage");
    }
}
