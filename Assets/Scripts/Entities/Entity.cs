using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

[RequireComponent(typeof(SpriteLibrary))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour, IMovable, ICanAttack
{
    public SpriteLibraryAsset[] spriteLibraries;
    [HideInInspector] public Direction dir = Direction.Down;
    [HideInInspector] public bool isMoving = false;
    public event Action onAttackHit;
    protected SpriteLibrary spriteLib;
    protected Animator anim;
    protected Rigidbody2D rb;
    protected Health health;

    void Awake()
    {
        spriteLib = GetComponent<SpriteLibrary>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
    }

    void OnEnable()
    {
        if (health) health.onDeath += Death;
    }

    void OnDisable()
    {
        if (health) health.onDeath -= Death;
    }

    void Update()
    {
        spriteLib.spriteLibraryAsset = spriteLibraries[(int)dir];
        anim.SetBool("Moving", isMoving);

        // Horizontal flip
        if ((dir == Direction.Left && transform.localScale.x > 0) || (dir != Direction.Left && transform.localScale.x < 0))
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    public void MoveToOverTime(Vector2 to, float time)
    {
        if (time == 0)
        {
            rb.MovePosition(to);
            return;
        }

        Vector2 from = rb.position;
        float dist = Vector2.Distance(from, to);
        float speed = dist / time;
        rb.velocity = (to - from).normalized * speed;
    }

    public void Attack()
    {
        anim.SetTrigger("Attack");
    }

    public void OnAttackAnimationHit()  // Animation event
    {
        onAttackHit?.Invoke();
    }

    public void Death() { anim.SetTrigger("Die"); }
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