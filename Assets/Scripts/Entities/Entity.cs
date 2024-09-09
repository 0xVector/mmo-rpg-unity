using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

/// <summary>
/// Represents an abstract entity that can be moved and can attack.
/// Requires a <see cref="SpriteRenderer"/>, <see cref="SpriteLibrary"/>, <see cref="Animator"/>, and <see cref="Rigidbody2D"/> components.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SpriteLibrary))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour, IMovable, ICanAttack
{
    /// <summary>
    /// The sprite libraries for each direction.
    /// The order is the same as the <see cref="Direction"/> enum
    /// - Down, Up, Left, Right.
    /// </summary>
    public SpriteLibraryAsset[] spriteLibraries;

    /// <summary>
    /// The network ID (UUID) of the entity.
    /// </summary>
    [HideInInspector] public string netId;

    /// <summary>
    /// The direction the entity is facing.
    /// </summary>
    [HideInInspector] public Direction dir = Direction.Down;

    /// <summary>
    /// Whether the entity is moving.
    /// </summary>
    [HideInInspector] public bool isMoving = false;

    /// <summary>
    /// Whether the entity is dashing.
    /// </summary>
    [HideInInspector] public bool isDashing = false;

    /// <summary>
    /// Event that is triggered when the entity's attack actually hits.
    /// </summary>
    public event Action onAttackHit;

    /// <summary>
    /// The velocity to change to in the next fixed update.
    /// If null, the velocity will not be changed.
    /// </summary>
    protected Vector2? nextVel = null;

    /// <summary>
    /// The position to instantly move to in the next fixed update.
    /// If null, the position will not be changed.
    /// </summary>
    protected Vector2? nextInstantMove = null;

    protected SpriteRenderer sprite;
    protected SpriteLibrary spriteLib;
    protected Animator anim;
    protected Rigidbody2D rb;
    protected Health health;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        spriteLib = GetComponent<SpriteLibrary>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
    }

    void OnEnable()
    {
        if (health)
        {
            health.onDamage += OnDamage;
            health.onDeath += OnDeath;
        }
    }

    void OnDisable()
    {
        if (health)
        {
            health.onDamage -= OnDamage;
            health.onDeath -= OnDeath;
        }
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

    /// <summary>
    /// Move the entity to a position over time.
    /// </summary>
    /// <param name="to">The position to move to.</param>
    /// <param name="time>The time in seconds for the move to finish.</param>
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

    /// <summary>
    /// Make the entity attack.
    /// </summary>
    public void Attack()
    {
        anim.SetTrigger("Attack");
    }

    /// <summary>
    /// Called when the entity takes damage.
    /// </summary>
    public abstract void OnDamage();

    /// <summary>
    /// Called when the entity dies.
    /// </summary>
    public void OnDeath()
    {
        anim.SetTrigger("Death");
        rb.simulated = false;
    }

    /// <summary>
    /// Animation event that is triggered when the attack animation hits.
    /// </summary>
    public void OnAttackAnimationHit()  // Animation event
    {
        onAttackHit?.Invoke();
    }

    /// <summary>
    /// Animation event that is triggered when the death animation finishes.
    /// </summary>
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

/// <summary>
/// Represents the direction of an object.
/// </summary>
public enum Direction
{
    Down,
    Up,
    Left,
    Right
}