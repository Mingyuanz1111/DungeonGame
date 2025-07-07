using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health;

    public bool isAutoHeal = false;
    public float autoHealPerSec = 5f;

    void Awake()
    {
        health = maxHealth;
    }

    void Update()
    {
        if (isAutoHeal)
        {
            Heal(5f * Time.deltaTime);
        }
    }

    public void Heal(float heal)
    {
        health = Mathf.Clamp(health + heal, 0, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        health = Mathf.Clamp(health - damage, 0, maxHealth);
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
