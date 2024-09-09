using UnityEngine;

[RequireComponent(typeof(Health))]
public class Player : Entity
{
    public Color hurtColor;

    public override void OnDamage()
    {
        sprite.color = hurtColor;
        Invoke("ResetColor", 0.1f);
    }

    void ResetColor() { sprite.color = Color.white; }
}