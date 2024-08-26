namespace MessageData
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

    public sealed class HeartbeatData : MessageData { }
    public sealed class EntitySpawnData : PositionedMessageData
    {
        public EntityType entity { get; set; }
    }
    public sealed class EntityDespawnData : MessageData { }
    public sealed class EntityMoveData : PositionedMessageData
    {
        public float speed { get; set; }
    }
    public sealed class PlayerUpdateData : MessageData
    {
        public Direction facing { get; set; }
        public bool isRunning { get; set; }
        public bool isAttacking { get; set; }
    }
}