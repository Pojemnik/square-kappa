using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventsAdapter : MonoBehaviour
{
    [SerializeField]
    private bool callDeathEvent;

    private Health health;
    private Rigidbody rb;
    private Vector3 startPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
        health.deathEvent.AddListener(OnPlayerDeath);
        health.damageEvent.AddListener(delegate { OnPlayerDamage(); });
        EventManager.Instance.AddListener("GameReloaded", OnGameReolad);
        startPosition = rb.position;
    }

    private void MovePlayerToStartPosition()
    {
        rb.isKinematic = true;
        rb.position = startPosition;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false;
    }

    private void OnGameReolad()
    {
        health.Heal();
        MovePlayerToStartPosition();
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
