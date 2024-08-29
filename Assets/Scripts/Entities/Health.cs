using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public int maxHealth = 1;
    public int health;
    public Action onDamage;
    public Action onDeath;

    public bool Alive { get => health > 0; }

    public void TakeDamage(int damage)
    {
        health -= damage;
        onDamage?.Invoke();
        if (health <= 0) Kill();
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
