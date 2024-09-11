using Entities;

namespace WebSockets.OutMessageData
{
    /// <summary>
    /// Represents abstract outgoing message data.
    /// </summary>
    public abstract class MessageData
    {
        public string id { get; set; }
    }

    /// <summary>
    /// Represents the player join message data.
    /// This is sent when the client wants to register the new player with the server.
    /// </summary>
    /// <param name="playerName">The name of the player.</param>
    public sealed class JoinData
    {
        public string playerName { get; set; }
    }

    /// <summary>
    /// Represents the player leave message data.
    /// This is sent when the client wants to leave the server.
    /// </summary>
    public sealed class LeaveData : MessageData { }

    /// <summary>
    /// Represents the heartbeat message data.
    /// </summary>
    public sealed class HeartbeatData : MessageData { }

    /// <summary>
    /// Represents the player spawn message data.
    /// This is sent when the client wants to spawn the player.
    /// </summary>
    public sealed class SpawnData : MessageData { }

    /// <summary>
    /// Represents the player move message data.
    /// It is used to report the player's movement to the server.
    /// </summary>
    /// <param name="x">The x position of the player.</param>
    /// <param name="y">The y position of the player.</param>
    public sealed class MoveData : MessageData
    {
        public float x { get; set; }
        public float y { get; set; }
    }

    /// <summary>
    /// Represents the player update message data.
    /// It is used to report the current state of the player to the server.
    /// </summary>
    /// <param name="dir">The direction the player is facing.</param>
    /// <param name="isMoving">Whether the player is moving.</param>
    /// <param name="isDashing">Whether the player is dashing.</param>
    public sealed class UpdateData : MessageData
    {
        public Direction dir { get; set; }
        public bool isMoving { get; set; }
        public bool isDashing { get; set; }
    }

    /// <summary>
    /// Represents the player attack message data.
    /// It is used to report the player's attack (the animation) to the server.
    /// </summary>
    public sealed class AttackData : MessageData { }

    /// <summary>
    /// Represents the player hit message data.
    /// It is used to report the player's hit to the server.
    /// Each hit message reports one hit.
    /// </summary>
    /// <param name="targetId">The UUID of the target entity.</param>
    public sealed class HitData : MessageData
    {
        public string targetId { get; set; }
    }
}