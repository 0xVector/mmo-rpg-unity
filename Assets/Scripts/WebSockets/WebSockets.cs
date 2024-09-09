using System.Collections.Generic;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

using NativeWebSocket;

/// <summary>
/// Manages the WebSocket connection to the server.
/// Allows sending messages to the server
/// and binding event handlers for incoming messages.
/// Uses the <see cref="NativeWebSocket"/> package.
/// Requires a <see cref="GameManager"/> component.
/// </summary>
[RequireComponent(typeof(GameManager))]
public class WebSockets : MonoBehaviour
{
    /// <summary>
    /// The WebSocket server address.
    /// </summary>
    public string SERVER_ADDRESS = "ws://localhost:3000";

    WebSocket websocket;
    Dictionary<string, Action<string>> eventHandlers;
    GameManager manager;
    JsonSerializerOptions options = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseLower) } };

    /// <summary>
    /// Binds a handler to an event.
    /// </summary>
    /// <param name="event">The event name.</param>
    /// <param name="handler">The handler.</param>
    public void bindHandler(string @event, Action<string> handler) { eventHandlers[@event] = handler; }

    /// <summary>
    /// Sends a message to the server.
    /// </summary>
    /// <param name="eventName">The event name.</param>
    /// <param name="data">The event data.</param>
    public async void SendWSMessage(string eventName, object data)
    {
        if (websocket.State == WebSocketState.Open)
        {
            string m = JsonSerializer.Serialize(new { @event = eventName, data }, options);
            await websocket.SendText(m);
        }
    }

    /// <summary>
    /// Closes the WebSocket connection.
    /// </summary>
    public async void CloseConnection() { await websocket.Close(); }

    async void Awake()
    {
        websocket = new WebSocket(SERVER_ADDRESS);
        eventHandlers = new Dictionary<string, Action<string>>();
        manager = GetComponent<GameManager>();

        websocket.OnOpen += () => Debug.Log("Connected!");
        websocket.OnError += (e) => Debug.Log("Error! " + e);
        websocket.OnClose += (e) => Debug.Log("Connection closed!");

        websocket.OnMessage += (bytes) =>
        {
            var rawMessage = System.Text.Encoding.UTF8.GetString(bytes);
            var message = JsonSerializer.Deserialize<WebSocketMessage>(rawMessage);
            eventHandlers.TryGetValue(message.@event, out Action<string> handler);
            if (handler != null) handler(JsonSerializer.Serialize(message.data));  // hacky way to bypass unknown type
        };

        Debug.Log("Connecting to the server...");
        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    void OnApplicationQuit() { CloseConnection(); }
}

/// <summary>
/// Represents a WebSocket message.
/// </summary>
class WebSocketMessage
{
    public string @event { get; set; }
    public dynamic data { get; set; }
}