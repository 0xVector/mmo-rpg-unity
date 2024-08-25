using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    public Direction dir = Direction.Down;
    public SpriteLibraryAsset[] spriteLibraries;
    Rigidbody2D rb;
    SpriteRenderer sprite;
    SpriteLibrary spriteLib;
    Animator anim;
    Vector2 move = new Vector2(0, 0);


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        spriteLib = GetComponent<SpriteLibrary>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        move = new(moveX, moveY);

        if (moveX > 0) dir = Direction.Right;
        else if (moveX < 0) dir = Direction.Left;
        else if (moveY < 0) dir = Direction.Down;
        else if (moveY > 0) dir = Direction.Up;

        if ((dir == Direction.Left && transform.localScale.x > 0) || (dir != Direction.Left && transform.localScale.x < 0)) {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        spriteLib.spriteLibraryAsset = spriteLibraries[(int)dir];
    }

    void FixedUpdate()
    {
        anim.SetBool("Running", move.magnitude > 0);

        move.Normalize();
        move *= speed;
        rb.velocity = move;
    }
}

public enum Direction
{
    Down,
    Up,
    Left,
    Right
}
