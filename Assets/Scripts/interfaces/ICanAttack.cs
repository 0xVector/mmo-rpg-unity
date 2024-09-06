using System;

public interface ICanAttack
{
    void Attack();
    public event Action onAttackHit;
}
