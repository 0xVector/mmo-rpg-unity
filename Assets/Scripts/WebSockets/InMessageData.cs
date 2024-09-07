namespace InMessageData
{

    // The order of the enum values determines the order in the inspector field
    public enum EntityType
    {
        Player,
        Slime
    }

    public abstract class MessageData
    {
        public string id { get; set; }
    }


    public abstract class PositionedMessageData : MessageData
    {
        public float x { get; set; }
        public float y { get; set; }
    }

    public sealed class JoinData : MessageData { }
    public sealed class LeaveData : MessageData { }

    public sealed class HeartbeatData : MessageData { }
    public sealed class EntitySpawnData : PositionedMessageData
    {
        public EntityType entity { get; set; }
    }
    public sealed class EntityDespawnData : MessageData { }
    public sealed class EntityMoveData : PositionedMessageData
    {
        public float time { get; set; }
    }
    public sealed class EntityUpdateData : MessageData
    {
        public Direction dir { get; set; }
        public bool isMoving { get; set; }
        public bool isDashing { get; set; }
    }

    public sealed class EntityAttackData : MessageData { }
    public sealed class EntityDeathData : MessageData { }
}