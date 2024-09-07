using UnityEngine;

public class Slime : Entity
{
    protected override void Update() {
        // Client-side rotation
        float x = rb.velocity.x;
        if (x > 0) dir = Direction.Right;
        else if (x < 0) dir = Direction.Left;

        base.Update();
    }
}