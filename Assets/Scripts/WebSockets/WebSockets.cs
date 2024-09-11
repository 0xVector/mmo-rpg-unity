using System.Collections.Generic;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

using NativeWebSocket;

namespace WebSockets
{

    /// <summary>
    /// Manages the WebSocket connection to the server.
    /// Allows sending messages to the server
    /// and binding event handlers for incoming messages.
    /// Uses the <see cref="NativeWebSocket"/> package.
    /// Requires a <see cref="GameManager"/> component.
    /// </summary>
    [RequireComponent(typeof(GameManager))]
    public class WebSocketManager : MonoBehaviour
    {
        /// <summary>
        /// Exposes the WebSocket connection state.
        /// </summary>
        /// <value>The current <see cref="WebSocketState"/> of the connection.</value>
        public WebSocketState State => websocket?.State ?? WebSocketState.Closed;

        /// <summary>
        /// Returns whether the WebSocket connection is open.
        /// </summary>
        /// <value>True if the connection is open, false otherwise.</value>
        public bool Connected => State == WebSocketState.Open;

        /// <summary>
        /// The WebSocket server address that will be used by <see cref="Connect"/>.
        /// Changing this value will not affect the current connection.
        /// </summary>
        public string ServerAddress = "ws://localhost:3000";

        private static readonly JsonSerializerOptions options = new() { Converters = { new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseLower) } };
        private WebSocket websocket;
        private Dictionary<string, Action<string>> eventHandlers;
        private GameManager manager;

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
        /// Connects to the WebSocket server.
        /// Uses the value from <see cref="ServerAddress"/>.
        /// If a connection is already open, it will be closed.
        /// </summary>
        public async void Connect()
        {
            if (Connected) CloseConnection();

            websocket = new WebSocket(ServerAddress);
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

            Debug.Log($"Connecting to the server... ({ServerAddress})");
            await websocket.Connect();
        }

        /// <summary>
        /// Closes the WebSocket connection.
        /// </summary>
        public async void CloseConnection()
        {
            if (Connected) await websocket.Close();
        }

        void Awake()
        {
            eventHandlers = new Dictionary<string, Action<string>>();
            manager = GetComponent<GameManager>();
        }

        void Update()
        {
            if (Connected)
            {
#if !UNITY_WEBGL || UNITY_EDITOR
                websocket.DispatchMessageQueue();
#endif
            }
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
}