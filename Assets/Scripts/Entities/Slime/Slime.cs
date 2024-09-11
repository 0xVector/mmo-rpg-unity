using UnityEngine;

namespace Entities.Slime
{
    /// <summary>
    /// Represents a slime entity.
    /// Requires a <see cref="Health"/> component.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class Slime : Entity
    {
        public override void OnDamage()
        {
            anim.SetTrigger("Damaged");
        }
    }
}