using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.Animation;

[RequireComponent(typeof(SpriteLibrary))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour
{
    public SpriteLibraryAsset[] spriteLibraries;
    public bool isControllable = false;
    [HideInInspector] public Direction dir = Direction.Down;
    [HideInInspector] public bool isRunning = false;
    public Action onAttackAnimationHit;
    SpriteLibrary spriteLib;
    Animator anim;
    Health health;

    void Awake()
    {
        spriteLib = GetComponent<SpriteLibrary>();
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();
    }

    void OnEnable()
    {
        health.onDeath += PlayDeath;
    }

    void OnDisable()
    {
        health.onDeath -= PlayDeath;
    }

    void Update()
    {
        spriteLib.spriteLibraryAsset = spriteLibraries[(int)dir];
        anim.SetBool("Running", isRunning);

        // Horizontal flip
        if ((dir == Direction.Left && transform.localScale.x > 0) || (dir != Direction.Left && transform.localScale.x < 0))
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    public void PlayAttack() { anim.SetTrigger("Attack"); }

    public void OnAttackAnimationHit()  // Animation event
    {
        onAttackAnimationHit?.Invoke();
        // if (isControllable) attack.ProcessAttack();
    }

    public void PlayDeath() { anim.SetTrigger("Die"); }
    public void OnDeathFinished()
    {
        Destroy(gameObject);
    }
}

public enum Direction
{
    Down,
    Up,
    Left,
    Right
}
