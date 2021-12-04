using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IntEvent : UnityEvent<int> {}

[System.Serializable]
public class DamageEvent : UnityEvent<DamageInfo> {}

[RequireComponent(typeof(Collider))]
public class Health : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField]
    private int maxHealth;
    [SerializeField]
    private float collisionDamageForceTreshold;
    [SerializeField]
    private float collisionDamageForceMultipler;
    [SerializeField]
    [Tooltip("Amount of damage subrtacted from every hit")]
    [Min(0)]
    private int armor;

    [Header("Particles")]
    [SerializeField]
    private GameObject hitParticlePrefab;
    [SerializeField]
    private GameObject destructionParticlePrefab;

    [Header("Events")]
    public UnityEvent deathEvent;
    public IntEvent healthChangeEvent;
    public DamageEvent damageEvent;

    private int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void DealDamageFromNowhere(int amount)
    {
        Damaged(new DamageInfo(amount, Vector3.forward, Vector3.forward, Vector3.forward));
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
        if (hitParticlePrefab != null)
        {
            GameObject particle = Instantiate(hitParticlePrefab, info.position, Quaternion.LookRotation(info.normal));
            particle.transform.localScale = transform.localScale;

        }
        currentHealth -= info.amount;
        healthChangeEvent.Invoke(currentHealth);
        damageEvent.Invoke(info);
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            if (destructionParticlePrefab != null)
            {
                GameObject particle = Instantiate(destructionParticlePrefab, transform.position, transform.rotation);
                particle.transform.localScale = transform.localScale;
            }
            deathEvent.Invoke();
        }
    }

    public void Heal()
    {
        currentHealth = maxHealth;
        healthChangeEvent.Invoke(currentHealth);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        healthChangeEvent.Invoke(currentHealth);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.impulse.magnitude > collisionDamageForceTreshold)
        {
            int damage = (int)((collision.impulse.magnitude - collisionDamageForceTreshold) * collisionDamageForceMultipler);
            Vector3 direction = transform.position - collision.gameObject.transform.position;
            ContactPoint contactPoint = collision.GetContact(0);
            DamageInfo info = new DamageInfo(damage, direction, contactPoint.point, contactPoint.normal);
            Damaged(info);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("HealthPack") && currentHealth != maxHealth)
        {
            HealthPackController packController = other.gameObject.GetComponent<HealthPackController>();
            Heal(packController.HealAmount);
            packController.Consume();
        }
    }
}

[System.Serializable]
public struct DamageInfo
{
    public int amount;
    public Vector3 direction;
    public Vector3 position;
    public Vector3 normal;

    public DamageInfo(int _amount, Vector3 _direction, Vector3 _position, Vector3 _normal)
    {
        amount = _amount;
        direction = _direction;
        position = _position;
        normal = _normal;
    }
}
