using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

using InMessageData;
using OutJoinData = OutMessageData.JoinData;
using OutSpawnData = OutMessageData.SpawnData;
using OutHeartbeatData = OutMessageData.HeartbeatData;

/// <summary>
/// Provides handlers for incoming messages and manages the existing entities.
/// </summary>
public class GameManager : MonoBehaviour
{
    public GameObject[] entityPrefabs;
    public GameObject controlledPlayerPrefab;
    public string playerName = "Tester";
    [HideInInspector] public string id;
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
        ws.bindHandler("entity-update", EntityUpdate);
        ws.bindHandler("entity-attack", EntityAttack);
        ws.bindHandler("entity-damage", EntityDamage);

        // Start by registering
        // Invoke("Register", 1f);
        // Invoke("SpawnSelf", 2f);
    }

    public void Register()
    {
        Debug.Log("Registering...");
        ws.SendWSMessage("join", new OutJoinData { playerName = playerName });
    }

    public void SpawnSelf()
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
        if (data.id == id && data.entity == EntityType.Player) prefab = controlledPlayerPrefab;

        var instance = Instantiate(prefab, new Vector2(data.x, data.y), Quaternion.identity);
        instance.GetComponent<Entity>().netId = data.id; // TODO: ???

        // Remove entity on death
        instance.TryGetComponent(out Health health);
        health.onDeath += () => entities.Remove(data.id);

        entities[data.id] = instance;
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
        if (data.id == id) return;  // Ignore self updates (for now)

        var entity = entities[data.id];
        entity.TryGetComponent(out IMovable movable);
        if (movable == null) return;

        movable.MoveToOverTime(new Vector2(data.x, data.y), data.time);
        Debug.Log($"Move {(data.time == 0 ? "[INSTANT]" : "")} {data.id} by {Vector2.Distance(new Vector2(data.x, data.y), entity.transform.position)} to {new Vector2(data.x, data.y)} ({entity.transform.position.x},{entity.transform.position.y}) t={data.time}");
    }

    void EntityUpdate(string rawData)
    {
        var data = JsonSerializer.Deserialize<EntityUpdateData>(rawData, options);
        if (!entities.ContainsKey(data.id)) return;
        if (data.id == id) return;  // Ignore self updates (for now)

        var entity = entities[data.id].GetComponent<Entity>();
        if (entity == null) return;

        entity.dir = data.dir;
        entity.isMoving = data.isMoving;
        entity.isDashing = data.isDashing;
        entity.score = data.score;

        var health = entity.GetComponent<Health>();
        if (health) health.SetHealth(data.hp);
        Debug.Log($"Update: facing={data.dir} moving={data.isMoving} dashing={data.isDashing} hp={data.hp} ({data.id})");
    }

    void EntityAttack(string rawData)
    {
        var data = JsonSerializer.Deserialize<EntityAttackData>(rawData);
        if (!entities.ContainsKey(data.id)) return;
        if (data.id == id) return;  // Ignore own attacks

        var entity = entities[data.id];
        entity.TryGetComponent(out ICanAttack attack);
        if (attack == null) return;

        attack.Attack();
        Debug.Log($"Attack {data.id}");
    }

    void EntityDamage(string rawData)
    {
        var data = JsonSerializer.Deserialize<EntityDamageData>(rawData);
        if (!entities.ContainsKey(data.id)) return;

        var entity = entities[data.id];
        entity.TryGetComponent(out IDamageable damageable);
        if (damageable == null) return;
        damageable.TakeDamage(data.damage);
        Debug.Log($"Damage {data.id} by {data.damage}");
    }
}