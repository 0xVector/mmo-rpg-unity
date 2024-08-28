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
    Player player;
    Updater updater;
    float lastAttackAt = 0;

    void Awake()
    {
        player = GetComponent<Player>();
        updater = GetComponent<Updater>();
    }

    void Update()
    {
        // Atack
        bool atack = Input.GetButtonDown("Fire1");
        if (atack && Time.time - lastAttackAt >= atackSpeed)
        {
            lastAttackAt = Time.time;
            player.PlayAttack();
            updater.sendAttack();
        }
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
