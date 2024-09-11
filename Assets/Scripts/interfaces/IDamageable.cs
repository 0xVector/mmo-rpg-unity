namespace Interfaces
{
    /// <summary>
    /// Represents an object that can take damage.
    /// </summary>
    interface IDamageable
    {
        /// <summary>
        /// Make the object take damage.
        /// </summary>
        /// <param name="damage">The amount of damage to give.</param>
        public void TakeDamage(float damage);

        /// <summary>
        /// Kill the object.
        /// </summary>
        public void Kill();
    }
}