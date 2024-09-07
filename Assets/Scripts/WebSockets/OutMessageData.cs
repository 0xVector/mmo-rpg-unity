namespace OutMessageData
{
    public abstract class MessageData
    {
        public string id { get; set; }
    }

    public sealed class JoinData
    {
        public string playerName { get; set; }
    }

    public sealed class LeaveData : MessageData { }
    public sealed class HeartbeatData : MessageData { }
    public sealed class SpawnData : MessageData { }

    public sealed class MoveData : MessageData
    {
        public float x { get; set; }
        public float y { get; set; }
    }

    public sealed class UpdateData : MessageData
    {
        public Direction dir { get; set; }
        public bool isMoving { get; set; }
        public bool isDashing { get; set; }
    }

    public sealed class AttackData : MessageData { }
    public sealed class HitData : MessageData
    {
        public string targetId { get; set; }
    }
}