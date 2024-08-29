using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    public float atackSpeed = 0.5f;
    public float attackRange = 0.5f;
    public Transform[] attackPoints;
    public LayerMask attackableLayers;
    public Action onAttack;
    Player player;
    float lastAttackAt = 0;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    void OnEnable()
    {
        player.onAttackAnimationHit += ProcessAttack;
    }

    void OnDisable()
    {
        player.onAttackAnimationHit -= ProcessAttack;
    }

    void Update()
    {
        // Atack
        bool atack = Input.GetButtonDown("Fire1");
        if (atack && Time.time - lastAttackAt >= atackSpeed)
        {
            lastAttackAt = Time.time;
            DoAttack();
        }
    }

    void DoAttack()
    {
        player.PlayAttack();
        onAttack?.Invoke();
    }

    public void ProcessAttack()
    {
        Direction dir = player.dir;
        Transform point = attackPoints[(int)dir];
        Collider2D[] hits = Physics2D.OverlapCircleAll(point.position, attackRange, attackableLayers);
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
