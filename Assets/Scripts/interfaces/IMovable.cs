using UnityEngine;

namespace Interfaces
{
    /// <summary>
    /// Represents an object that can be moved to a position over time.
    /// Usually used by the <see cref="GameManager"/> to move objects around based on incoming server messages.
    /// </summary>
    interface IMovable
    {
        /// <summary>
        /// Move the object to a position over time.
        /// </summary>
        /// <param name="to">The position to move to.</param>
        /// <param name="time">The time in seconds for the move to finish.</param>
        void MoveToOverTime(Vector2 to, float time);
    }
}