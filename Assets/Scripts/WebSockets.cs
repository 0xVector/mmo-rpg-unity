using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using UnityEngine;

using NativeWebSocket;

public class WebSockets : MonoBehaviour
{
    public string SERVER_ADDRESS = "ws://localhost:3000";
    WebSocket websocket;
    Dictionary<string, Action<string>> eventHandlers;

    public void bindHandler(string @event, Action<string> handler) { eventHandlers[@event] = handler; }

    public async void SendWebSocketMessage(string event_name, object data)
    {
        if (websocket.State == WebSocketState.Open)
        {
            string m = JsonSerializer.Serialize(new { @event = event_name, data });
            await websocket.SendText(m);
        }
    }

    public async void CloseConnection() { await websocket.Close(); }

    async void Awake()
    {
        websocket = new WebSocket(SERVER_ADDRESS);
        eventHandlers = new Dictionary<string, Action<string>>();

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

    // Update is called once per frame
    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    void OnApplicationQuit() { CloseConnection(); }
}

class WebSocketMessage
{
    public string @event { get; set; }
    public dynamic data { get; set; }
}