using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IntEvent : UnityEvent<int> {}

public class Health : MonoBehaviour
{
    public int maxHealth;
    public UnityEvent deathEvent;
    public IntEvent healthChangeEvent;

    private int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Damaged(int amount)
    {
        currentHealth -= amount;
        healthChangeEvent.Invoke(currentHealth);
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            deathEvent.Invoke();
        }
    }
}
