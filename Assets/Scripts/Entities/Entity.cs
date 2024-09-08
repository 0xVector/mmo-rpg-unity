using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

[RequireComponent(typeof(SpriteLibrary))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour, IMovable, ICanAttack
{
    public SpriteLibraryAsset[] spriteLibraries;
    [HideInInspector] public string netId;
    [HideInInspector] public Direction dir = Direction.Down;
    [HideInInspector] public bool isMoving = false;
    [HideInInspector] public bool isDashing = false;
    public event Action onAttackHit;
    protected Vector2? nextVel = null;
    protected Vector2? nextInstantMove = null;
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

    protected virtual void OnEnable()
    {
        if (health) health.onDeath += Death;
    }

    protected virtual void OnDisable()
    {
        if (health) health.onDeath -= Death;
    }

    protected virtual void Update()
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
            nextInstantMove = to;
            nextVel = Vector2.zero;  // Stop moving
        }
        else
        {
            Vector2 from = rb.position;
            float dist = Vector2.Distance(from, to);
            float speed = dist / time;
            nextVel = (to - from).normalized * speed;
        }
    }

    public void Attack()
    {
        anim.SetTrigger("Attack");
    }

    public void OnAttackAnimationHit()  // Animation event
    {
        onAttackHit?.Invoke();
    }

    public void Death()
    {
        anim.SetTrigger("Death");
        rb.simulated = false;
    }
    public void OnDeathFinished()
    {
        Destroy(gameObject);
    }

    protected virtual void FixedUpdate()
    {
        if (nextInstantMove is Vector2 move)
        {
            rb.MovePosition(move);
            nextInstantMove = null;
        }
        if (nextVel is Vector2 vel)
        {
            rb.velocity = vel;
            nextVel = null;
        }
    }
}

public enum Direction
{
    Down,
    Up,
    Left,
    Right
}