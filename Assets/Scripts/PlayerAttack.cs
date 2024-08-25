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

    Animator anim;
    PlayerController playerController;
    float lastAttackAt = 0;

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        // Atack
        bool atack = Input.GetButtonDown("Fire1");
        if (atack && Time.time - lastAttackAt >= atackSpeed)
        {
            lastAttackAt = Time.time;
            anim.SetTrigger("Attack");
        }
    }

    public void AttackEvent()
    {
        Direction dir = playerController.dir;
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
