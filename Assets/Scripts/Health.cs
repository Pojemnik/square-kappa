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
    [Header("Parameters")]
    [SerializeField]
    private int maxHealth;
    [SerializeField]
    [Tooltip("Amount of damage subrtacted from every hit")]
    [Min(0)]
    private int armor;

    [Header("Events")]
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
        if(!enabled)
        {
            return;
        }
        info.amount -= armor;
        if(info.amount < 0)
        {
            info.amount = 0;
        }    
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
