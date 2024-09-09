using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class Movement : MonoBehaviour
{
    public float speed = 10.0f;
    public float dashSpeed = 20.0f;
    Entity entity;
    Rigidbody2D rb;
    Vector2 move = new Vector2(0, 0);

    void Awake()
    {
        entity = GetComponent<Entity>();
        rb = GetComponent<Rigidbody2D>();
        SetupCamera(transform);
    }

    void Update()
    {
        // Movement
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        move = new(moveX, moveY);
        entity.isMoving = move.magnitude > 0;

        // Direction
        Direction dir = entity.dir;
        if (moveX > 0) dir = Direction.Right;
        else if (moveX < 0) dir = Direction.Left;
        else if (moveY < 0) dir = Direction.Down;
        else if (moveY > 0) dir = Direction.Up;
        entity.dir = dir;

        // Dash
        entity.isDashing = Input.GetButton("Dash");
    }

    void FixedUpdate()
    {
        move.Normalize();
        move *= entity.isDashing ? dashSpeed : speed;
        rb.velocity = move;
    }

    void SetupCamera(Transform target)
    {
        Follow camera = FindObjectOfType<Follow>();
        camera.target = target;
    }
}