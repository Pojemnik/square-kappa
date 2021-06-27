using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IntEvent : UnityEvent<int> {}

[System.Serializable]
public class DamageEvent : UnityEvent<DamageInfo> {}

public class Health : MonoBehaviour
{
    public int maxHealth;
    public UnityEvent deathEvent;
    public IntEvent healthChangeEvent;
    public DamageEvent damageEvent;

    private int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Damaged(DamageInfo info)
    {
        currentHealth -= info.amount;
        healthChangeEvent.Invoke(currentHealth);
        damageEvent.Invoke(info);
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            deathEvent.Invoke();
        }
    }
}

[System.Serializable]
public struct DamageInfo
{
    public int amount;
    public Vector3 direction;

    public DamageInfo(int _amount, Vector3 _direction)
    {
        amount = _amount;
        direction = _direction;
    }
}
