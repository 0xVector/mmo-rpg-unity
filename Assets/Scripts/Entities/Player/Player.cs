using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Player : MonoBehaviour
{
    public SpriteLibraryAsset[] spriteLibraries;
    [HideInInspector] public Direction dir = Direction.Down;
    [HideInInspector] public bool isRunning = false;
    public bool isControllable = false;
    [HideInInspector] public bool isDead = false;
    SpriteLibrary spriteLib;
    Animator anim;
    PlayerController controller;
    PlayerAttack attack;

    void Awake()
    {
        spriteLib = GetComponent<SpriteLibrary>();
        anim = GetComponent<Animator>();
        if (isControllable) controller = GetComponent<PlayerController>();
        if (isControllable) attack = GetComponent<PlayerAttack>();
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

    public void OnAttack()  // Animation event
    {
        if (isControllable) attack.ProcessAttack();
    }

    public void PlayDeath()
    {
        anim.SetTrigger("Die");
        isDead = true;
        DisableControl();
    }
    public void OnDeathFinished()
    {
        Destroy(gameObject);
    }

    void DisableControl()
    {
        if (!isControllable) return;
        attack.enabled = false;
        controller.enabled = false;
    }
}

public enum Direction
{
    Down,
    Up,
    Left,
    Right
}
