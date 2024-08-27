using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    Player player;
    Rigidbody2D rb;
    Vector2 move = new Vector2(0, 0);


    void Awake()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Movement
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        move = new(moveX, moveY);
        player.isRunning = move.magnitude > 0;

        // Direction
        Direction dir = player.dir;
        if (moveX > 0) dir = Direction.Right;
        else if (moveX < 0) dir = Direction.Left;
        else if (moveY < 0) dir = Direction.Down;
        else if (moveY > 0) dir = Direction.Up;
        player.dir = dir;

        // Horizontal flip
        if ((dir == Direction.Left && transform.localScale.x > 0) || (dir != Direction.Left && transform.localScale.x < 0))
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    void FixedUpdate()
    {
        move.Normalize();
        move *= speed;
        rb.velocity = move;
    }
}