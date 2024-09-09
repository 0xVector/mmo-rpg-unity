using UnityEngine;

/// <summary>
/// Represents the player entity.
/// Requires a <see cref="Health"/> component.
/// </summary>
[RequireComponent(typeof(Health))]
public class Player : Entity
{
    /// <summary>
    /// The color to blink when the player takes damage.
    /// </summary>
    public Color hurtColor;

    public override void OnDamage()
    {
        sprite.color = hurtColor;
        Invoke("ResetColor", 0.1f);
    }

    void ResetColor() { sprite.color = Color.white; }
}