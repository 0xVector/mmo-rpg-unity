using System;

/// <summary>
/// Represents an object that can attack.
/// </summary>
public interface ICanAttack
{
    /// <summary>
    /// Do the attack.
    /// </summary>
    void Attack();

    /// <summary>
    /// Event that is triggered when the attack actually hits
    /// (eg. the animation hits the target).
    /// </summary>
    public event Action onAttackHit;
}
