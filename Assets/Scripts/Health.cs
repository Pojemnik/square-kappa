using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int maxHealth;
    public UnityEvent deathEvent;
    public UnityEvent damageEvent;

    private int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Damaged(int amount)
    {
        currentHealth -= amount;
        damageEvent.Invoke();
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            deathEvent.Invoke();
        }
    }
}
