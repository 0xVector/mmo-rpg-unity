using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using UnityEngine;

using NativeWebSocket;

public class WebSockets : MonoBehaviour
{
    WebSocket websocket;
    public string playerName = "Adam";

    // Start is called before the first frame update
    async void Start()
    {
        websocket = new WebSocket("ws://localhost:3000");

        websocket.OnOpen += () => Debug.Log("Connected!");
        websocket.OnError += (e) => Debug.Log("Error! " + e);
        websocket.OnClose += (e) => Debug.Log("Connection closed!");
        websocket.OnMessage += (bytes) =>
        {
            Debug.Log("Message: " + bytes.ToString());
            // getting the message as a string
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("MessageText: " + message);
        };

        // Keep sending messages at every 0.3s
        Invoke("Register", 1f);
        // InvokeRepeating("SendHeartbeat", 2f, 1f);

        // waiting for messages
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

    void Register()
    {
        SendWebSocketMessage("join", new { playerName });
        Debug.Log("Registering...");
    }

    void SendHeartbeat()
    {
        SendWebSocketMessage("heartbeat", new { id = "ID" });
    }

    async void SendWebSocketMessage(string event_name, object data)
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            // await websocket.Send(new byte[] { 10, 20, 30 });

            // Sending plain text
            string m = JsonSerializer.Serialize(new { @event = event_name, data });
            Debug.Log("Sending: " + m);
            await websocket.SendText(m);
        }
    }

    async void OnApplicationQuit()
    {
        await websocket.Close();
    }
}
