using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class GameManager : MonoBehaviour
{
    Dictionary<string, GameObject> entities;
    WebSockets ws;

    JsonSerializerOptions options = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseLower) } };

    void Awake()
    {
        entities = new Dictionary<string, GameObject>();
        ws = GetComponent<WebSockets>();
    }

    void Start()
    {
        // Bind handlers
        ws.bindHandler("leave", Leave);
        ws.bindHandler("heartbeat", Heartbeat);
        ws.bindHandler("entity-spawn", EntitySpawn);
        ws.bindHandler("entity-despawn", EntityDespawn);
        ws.bindHandler("entity-move", EntityMove);
        ws.bindHandler("player-update", PlayerUpdate);

        // Start by joining
        Invoke("Join", 1f);
    }

    void Join()
    {
        Debug.Log("Registering...");
        ws.SendWebSocketMessage("join", new { playerName = "Tester" });
    }

    void Leave(string rawData)
    {
        Debug.Log("Kicked by the server.");
        ws.CloseConnection();
    }

    void Heartbeat(string rawData)
    {
        var data = JsonSerializer.Deserialize<HeartbeatData>(rawData);
        ws.SendWebSocketMessage("heartbeat", new { data.id });
    }

    void EntitySpawn(string rawData)
    {
        var data = JsonSerializer.Deserialize<EntitySpawnData>(rawData, options);
        Debug.Log($"Spawn {data.entity} {data.id} at ({data.x}, {data.y})");
    }

    void EntityDespawn(string rawData)
    {
        var data = JsonSerializer.Deserialize<EntityDespawnData>(rawData);
        Debug.Log($"Despawn {data.id}");
    }

    void EntityMove(string rawData)
    {
        var data = JsonSerializer.Deserialize<EntityMoveData>(rawData);
        Debug.Log($"Move {data.id} to ({data.x},{data.y}) s={data.speed}");
    }

    void PlayerUpdate(string rawData)
    {
        var data = JsonSerializer.Deserialize<PlayerUpdateData>(rawData, options);
        Debug.Log($"Update: facing={data.facing} running={data.isRunning} attacking={data.isAttacking} ({data.id})");
    }
}

enum EntityType
{
    Player,
    Slime
}

abstract class MessageData
{
    public string id { get; set; }
}


abstract class PositionedMessageData : MessageData
{
    public float x { get; set; }
    public float y { get; set; }
}

sealed class HeartbeatData : MessageData { }
sealed class EntitySpawnData : PositionedMessageData
{
    public EntityType entity { get; set; }
}
sealed class EntityDespawnData : MessageData { }
sealed class EntityMoveData : PositionedMessageData
{
    public float speed { get; set; }
}
sealed class PlayerUpdateData : MessageData
{
    public Direction facing { get; set; }
    public bool isRunning { get; set; }
    public bool isAttacking { get; set; }
}

