namespace InMessageData
{

    /// <summary>
    /// Represents the type of entity.
    /// The order of the enum values determines the order in the inspector field.
    /// </summary>
    public enum EntityType
    {
        Player,
        Slime,
        SlimePurple,
    }

    /// <summary>
    /// Represents abstract incoming message data.
    /// </summary>
    /// <param name="id">The UUID of the entity this message refers to.</param>
    public abstract class MessageData
    {
        public string id { get; set; }
    }

    /// <summary>
    /// Represents incoming message data that contains a position.
    /// </summary>
    /// <param name="x">The x position of the entity.</param>
    /// <param name="y">The y position of the entity.</param>
    public abstract class PositionedMessageData : MessageData
    {
        public float x { get; set; }
        public float y { get; set; }
    }

    /// <summary>
    /// Represents the player join message data.
    /// </summary>
    public sealed class JoinData : MessageData { }

    /// <summary>
    /// Represents the player leave message data.
    /// </summary>
    public sealed class LeaveData : MessageData { }

    /// <summary>
    /// Represents the heartbeat message data.
    /// </summary>
    public sealed class HeartbeatData : MessageData { }

    /// <summary>
    /// Represents the entity spawn message data.
    /// </summary>
    /// <param name="entity">The type of entity to spawn.</param>
    public sealed class EntitySpawnData : PositionedMessageData
    {
        public EntityType entity { get; set; }
    }

    /// <summary>
    /// Represents the entity despawn message data.
    /// </summary>
    public sealed class EntityDespawnData : MessageData { }

    /// <summary>
    /// Represents the entity move message data.
    /// </summary>
    /// <param name="time">The time in seconds for the move to finish, counting from the time the message was received.</param>
    public sealed class EntityMoveData : PositionedMessageData
    {
        public float time { get; set; }
    }

    /// <summary>
    /// Represents the entity update message data.
    /// </summary>
    /// <param name="dir">The direction the entity is facing.</param>
    /// <param name="isMoving">Whether the entity is moving.</param>
    /// <param name="isDashing">Whether the entity is dashing.</param>
    /// <param name="hp">The current hp of the entity.</param>
    public sealed class EntityUpdateData : MessageData
    {
        public Direction dir { get; set; }
        public bool isMoving { get; set; }
        public bool isDashing { get; set; }
        public float hp { get; set; }
    }

    /// <summary>
    /// Represents the entity attack message data.
    /// 
    /// This message is sent by the server when an entity attacks (the attack animation starts).
    /// </summary>
    public sealed class EntityAttackData : MessageData { }

    /// <summary>
    /// Represents the entity damage message data.
    /// This message is sent by the server when an entity takes damage.
    /// </summary>
    /// <param name="damage">The amount of damage taken.</param>
    /// <param name="sourceX">The x position of the source of the damage.</param>
    /// <param name="sourceY">The y position of the source of the damage.</param>
    public sealed class EntityDamageData : MessageData
    {
        public float damage { get; set; }
        public float sourceX { get; set; }
        public float sourceY { get; set; }
    }
}