using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public float maxHealth = 1f;
    public float health = 1f;
    public Action onDamage;
    public Action onDeath;

    public bool Alive { get => health > 0; }

    public void TakeDamage(float damage)
    {
        health -= damage;
        onDamage?.Invoke();
        if (health <= 0) Kill();
    }

    public void SetHealth(float newHealth)
    {
        health = newHealth;
    }

    public void Kill()
    {
        health = 0;
        onDeath?.Invoke();
    }

    void Awake()
    {
        health = maxHealth;
    }
}
