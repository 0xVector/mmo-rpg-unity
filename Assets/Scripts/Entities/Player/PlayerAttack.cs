using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerAttack : MonoBehaviour
{

    public float atackCooldown = 0.5f;
    public float attackRange = 0.5f;
    public Transform[] attackPoints;
    public LayerMask attackableLayers;
    public Action onAttack;
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
        bool attack = Input.GetButtonDown("Fire1");
        if (attack && Time.time - lastAttackAt >= atackCooldown) {
            lastAttackAt = Time.time;
            Attack();
        }
    }

    void Attack() { 
        player.Attack();
        onAttack?.Invoke(); }

    public void ProcessAttack()
    {
        Direction dir = player.dir;
        Transform point = attackPoints[(int)dir];
        Collider2D[] hits = Physics2D.OverlapCircleAll(point.position, attackRange, attackableLayers);
        foreach (var hit in hits)
        {
            Entity entity = hit.GetComponent<Entity>();
            if (entity == null) continue;  // Ignore non-entities
            onHit?.Invoke(entity);
        }
        Debug.Log($"Attack hits: {hits.Length}");
    }

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