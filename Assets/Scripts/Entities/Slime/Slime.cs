using UnityEngine;

public class Slime : Entity
{
    protected override void OnEnable()
    {
        base.OnEnable();
        if (health) health.onDamage += OnDamage;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (health) health.onDamage -= OnDamage;
    }

    public void OnDamage()
    {
        anim.SetTrigger("Damaged");
    }
}