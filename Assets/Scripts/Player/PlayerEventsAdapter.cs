using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(ItemChanger))]
[RequireComponent(typeof(UnitShooting))]
public class PlayerEventsAdapter : MonoBehaviour
{
    [SerializeField]
    private bool callDeathEvent;

    private Health health;
    private ItemChanger itemChanger;
    private Rigidbody rb;
    private UnitShooting shooting;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
        health.deathEvent.AddListener(OnPlayerDeath);
        health.damageEvent.AddListener(delegate { OnPlayerDamage(); });
        EventManager.Instance.AddListener("GameReloaded", OnGameReload);
        startPosition = rb.position;
        startRotation = rb.rotation;
        shooting = GetComponent<UnitShooting>();
        itemChanger = GetComponent<ItemChanger>();
    }

    private void MovePlayerToStartPosition()
    {
        rb.isKinematic = true;
        rb.position = startPosition;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.rotation = startRotation;
        rb.isKinematic = false;
    }

    private void OnGameReload()
    {
        health.Heal();
        itemChanger.DropAndDestroyWeapon();
        itemChanger.ClearInventory();
        shooting.ResetAmmoAmount();
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
