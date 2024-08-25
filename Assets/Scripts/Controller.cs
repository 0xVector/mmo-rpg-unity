using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controllable : MonoBehaviour
{
    Vector2 target;
    Rigidbody2D rb;
    void Awake() {
        target = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 delta = target - (Vector2)transform.position - rb.velocity * Time.deltaTime;
    }

    public void Move(Vector2 to) {
        target = to;
    }
}
