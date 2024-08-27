using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

using InMessageData;
using OutJoinData = OutMessageData.JoinData;
using OutSpawnData = OutMessageData.SpawnData;
using OutHeartbeatData = OutMessageData.HeartbeatData;

public class GameManager : MonoBehaviour
{
    public GameObject[] entityPrefabs;
    public GameObject playerPrefab;
    
    [HideInInspector]
    public string id;
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
        ws.bindHandler("join", Join);
        ws.bindHandler("leave", Leave);
        ws.bindHandler("heartbeat", Heartbeat);
        ws.bindHandler("entity-spawn", EntitySpawn);
        ws.bindHandler("entity-despawn", EntityDespawn);
        ws.bindHandler("entity-move", EntityMove);
        ws.bindHandler("player-update", PlayerUpdate);

        // Start by registering
        Invoke("Register", 1f);
        Invoke("SpawnSelf", 2f);
    }

    void Register()
    {
        Debug.Log("Registering...");
        ws.SendWSMessage("join", new OutJoinData { playerName = "Tester" });
    }

    void SpawnSelf()
    {
        Debug.Log("Requesting spawn...");
        ws.SendWSMessage("spawn", new OutSpawnData { id = id });
    }

    void Join(string rawData)
    {
        var data = JsonSerializer.Deserialize<JoinData>(rawData);
        id = data.id;
        Debug.Log($"Registered with id: {id}");
    }

    void Leave(string rawData)
    {
        var data = JsonSerializer.Deserialize<LeaveData>(rawData);
        if (data.id != id) return;

        Debug.Log("Kicked by the server.");
        ws.CloseConnection();
    }

    void Heartbeat(string rawData)
    {
        var data = JsonSerializer.Deserialize<HeartbeatData>(rawData);
        if (data.id != id) return;
        ws.SendWSMessage("heartbeat", new OutHeartbeatData { id = id });
    }

    void EntitySpawn(string rawData)
    {
        var data = JsonSerializer.Deserialize<EntitySpawnData>(rawData, options);
        if (entities.ContainsKey(data.id)) return;

        GameObject prefab = entityPrefabs[(int)data.entity];

        // Own player spawn
        if (data.id == id && data.entity == EntityType.Player) prefab = playerPrefab;

        entities[data.id] = Instantiate(prefab, new Vector2(data.x, data.y), Quaternion.identity);
        Debug.Log($"Spawn {data.entity} {data.id} at {new Vector2(data.x, data.y)} ({data.x},{data.y})");
    }

    void EntityDespawn(string rawData)
    {
        var data = JsonSerializer.Deserialize<EntityDespawnData>(rawData);
        if (!entities.ContainsKey(data.id)) return;

        Destroy(entities[data.id]);
        Debug.Log($"Despawn {data.id}");
    }

    void EntityMove(string rawData)
    {
        var data = JsonSerializer.Deserialize<EntityMoveData>(rawData);
        if (!entities.ContainsKey(data.id)) return;

        var entity = entities[data.id];
        entity.TryGetComponent(out Rigidbody2D rb);
        if (rb != null) rb.MovePosition(new Vector2(data.x, data.y));
        Debug.Log($"Move {data.id} to {new Vector2(data.x, data.y)} ({data.x},{data.y}) s={data.speed}");
    }

    void PlayerUpdate(string rawData)
    {
        var data = JsonSerializer.Deserialize<PlayerUpdateData>(rawData, options);
        if (!entities.ContainsKey(data.id)) return;
        if (data.id == id) return;  // Ignore self updates (for now)

        var player = entities[data.id].GetComponent<Player>();

        player.dir = data.facing;
        player.isRunning = data.isRunning;
        if (data.isAttacking) player.PlayAttack();
        Debug.Log($"Update: facing={data.facing} running={data.isRunning} attacking={data.isAttacking} ({data.id})");
    }
}