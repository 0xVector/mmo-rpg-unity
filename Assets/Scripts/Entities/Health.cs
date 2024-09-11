using System;
using UnityEngine;

using Interfaces;

namespace Entities
{
    /// <summary>
    /// Manages the health of an object.
    /// Can take damage and die.
    /// </summary>
    public class Health : MonoBehaviour, IDamageable
    {
        /// <summary>
        /// The maximum health of the object.
        /// </summary>
        public float maxHealth = 1f;

        /// <summary>
        /// The current health of the object.
        /// </summary>
        public float health = 1f;

        /// <summary>
        /// Event that is triggered when the object takes damage.
        /// </summary>
        public Action onDamage;

        /// <summary>
        /// Event that is triggered when the object dies.
        /// </summary>
        public Action onDeath;

        /// <summary>
        /// Property that returns whether the object is alive.
        /// </summary>
        /// <value>True if the object is alive, false otherwise.</value>
        public bool Alive { get => health > 0; }

        /// <summary>
        /// Makes the object take damage.
        /// </summary>
        /// <param name="damage">The amount of damage to take.</param>
        public void TakeDamage(float damage)
        {
            health -= damage;
            onDamage?.Invoke();
            if (health <= 0) Kill();
        }

        /// <summary>
        /// Sets the health of the object.
        /// Ignores the maximum health.
        /// Doesn't trigger the onDamage event even if the new health is lower than the current health.
        /// Does trigger the onDeath event though if the new health is 0 or lower.
        /// </summary>
        /// <param name="newHealth">The new health of the object.</param>
        public void SetHealth(float newHealth)
        {
            health = newHealth;
            if (health <= 0) Kill();
        }

        /// <summary>
        /// Kills the object.
        /// </summary>
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
}