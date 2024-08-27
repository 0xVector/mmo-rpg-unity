using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Player : MonoBehaviour
{
    public Direction dir = Direction.Down;
    public bool isRunning = false;
    public bool isControllable = false;
    public SpriteLibraryAsset[] spriteLibraries;
    SpriteLibrary spriteLib;
    Animator anim;
    PlayerAttack playerAttack;

    void Awake()
    {
        spriteLib = GetComponent<SpriteLibrary>();
        anim = GetComponent<Animator>();
        if (isControllable) playerAttack = GetComponent<PlayerAttack>();
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

    public void Attack()  // Play attack animation
    {
        anim.SetTrigger("Attack");
    }

    public void OnAttack()  // Animation event
    {
        if (isControllable) playerAttack.ProcessAttack();
    }
}

public enum Direction
{
    Down,
    Up,
    Left,
    Right
}
