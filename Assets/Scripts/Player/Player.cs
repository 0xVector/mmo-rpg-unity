using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Player : MonoBehaviour
{
    public SpriteLibraryAsset[] spriteLibraries;
    [HideInInspector]
    public Direction dir = Direction.Down;
    [HideInInspector]
    public bool isRunning = false;
    public bool isControllable = false;
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

    public void PlayAttack() { anim.SetTrigger("Attack"); }

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
