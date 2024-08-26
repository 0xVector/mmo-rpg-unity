using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

using MessageData;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject[] entityPrefabs;
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
        if (entities.ContainsKey(data.id)) return;

        GameObject prefab = entityPrefabs[(int)data.entity];
        entities[data.id] = Instantiate(prefab, remapCoordinates(data.x, data.y), Quaternion.identity);
        Debug.Log($"Spawn {data.entity} {data.id} at {remapCoordinates(data.x, data.y)} ({data.x},{data.y})");
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
        if (rb != null) rb.MovePosition(remapCoordinates(data.x, data.y));
        Debug.Log($"Move {data.id} to {remapCoordinates(data.x, data.y)} ({data.x},{data.y}) s={data.speed}");
    }

    void PlayerUpdate(string rawData)
    {
        var data = JsonSerializer.Deserialize<PlayerUpdateData>(rawData, options);
        Debug.Log($"Update: facing={data.facing} running={data.isRunning} attacking={data.isAttacking} ({data.id})");
    }

    static Vector2 remapCoordinates(float x, float y)
    {
        float remapCoefficient = 56f;
        return new Vector2(x/remapCoefficient, y/remapCoefficient);
    }
}