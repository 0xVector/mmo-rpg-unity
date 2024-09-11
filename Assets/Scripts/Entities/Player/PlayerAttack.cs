using System;
using UnityEngine;

/// <summary>
/// Reacts to input and attacks.
/// Requires a <see cref="Player"/> component.
/// </summary>
[RequireComponent(typeof(Player))]
public class PlayerAttack : MonoBehaviour
{
    /// <summary>
    /// The cooldown between attacks in seconds.
    /// </summary>
    public float atackCooldown = 0.5f;

    /// <summary>
    /// The range of the attack in meters.
    /// </summary>
    public float attackRange = 0.5f;

    /// <summary>
    /// The three points where the attack colliders are placed.
    /// The order is: Down, Up, Side.
    /// The side is determined by the <see cref="Player.dir"/> and rotates with the player.
    /// </summary>
    public Transform[] attackPoints;

    /// <summary>
    /// The layers that can be attacked.
    /// </summary>
    public LayerMask attackableLayers;

    /// <summary>
    /// Event that is triggered when the player starts an attack.
    /// </summary>
    public Action onAttack;

    /// <summary>
    /// Event that is triggered when the player hits an entity with an attack.
    /// The entity hit is passed as an argument.
    /// </summary>
    public Action<Entity> onHit;

    float lastAttackAt = 0;
    Player player;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    void OnEnable()
    {
        player.onAttackHit += ProcessAttack;
    }

    void OnDisable()
    {
        player.onAttackHit -= ProcessAttack;
    }

    void Update()
    {
        // Atack
        bool attack = Input.GetButtonDown("Attack");
        if (attack && Time.time - lastAttackAt >= atackCooldown)
        {
            lastAttackAt = Time.time;
            Attack();
        }
    }

    void Attack()
    {
        player.Attack();
        onAttack?.Invoke();
    }

    /// <summary>
    /// Processes the attack, checking for entities hit.
    /// </summary>
    void ProcessAttack()
    {
        Direction dir = player.dir;
        Transform point = attackPoints[(int)dir];
        Collider2D[] hits = Physics2D.OverlapCircleAll(point.position, attackRange, attackableLayers);
        foreach (var hit in hits)
        {
            Entity entity = hit.GetComponent<Entity>();
            if (entity == null) continue;  // Ignore non-entities
            if (entity == player) continue;  // Ignore self
            onHit?.Invoke(entity);
        }
        Debug.Log($"Attack hits: {hits.Length}");
    }

    /// <summary>
    /// Draws the attack range gizmos for easier debugging.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        foreach (var attackPoint in attackPoints)
        {
            if (!attackPoint) continue;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}