using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RigidMovable : MonoBehaviour, IMovable
{
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
}