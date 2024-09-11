using NativeWebSocket;
using UnityEngine;
using UnityEngine.UIElements;

using WebSockets;

namespace UI
{

    /// <summary>
    /// Manages the main menu.
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        private VisualElement root;
        private TextField serverAddress;
        private Button connectButton;
        private Label connectionStatus;
        private TextField playerName;
        private Button registerButton;
        private Button spawnButton;

        private WebSocketManager ws;
        private GameManager manager;

        void OnEnable()
        {
            var managerObj = GameObject.Find("Manager");
            ws = managerObj.GetComponent<WebSocketManager>();
            manager = managerObj.GetComponent<GameManager>();

            root = GetComponent<UIDocument>().rootVisualElement;

            serverAddress = root.Q<TextField>("server-address");
            connectButton = root.Q<Button>("server-connect");
            connectionStatus = root.Q<Label>("connection-status");
            playerName = root.Q<TextField>("player-name");
            registerButton = root.Q<Button>("register-button");
            spawnButton = root.Q<Button>("spawn-button");

            serverAddress.value = ws.ServerAddress;
            playerName.value = manager.playerName;

            // Register event handlers
            connectButton.clicked += Connect;
            serverAddress.RegisterCallback<ChangeEvent<string>>(ChangeAddress);
            playerName.RegisterCallback<ChangeEvent<string>>(ChangePlayerName);
            registerButton.clicked += Register;
            spawnButton.clicked += Spawn;
        }

        void OnDisable()
        {
            connectButton.clicked -= Connect;
            serverAddress.UnregisterCallback<ChangeEvent<string>>(ChangeAddress);
            playerName.UnregisterCallback<ChangeEvent<string>>(ChangePlayerName);
            registerButton.clicked -= Register;
            spawnButton.clicked -= Spawn;
        }

        void Update()
        {
            connectionStatus.text = ws.State.ToString();
            connectButton.SetEnabled(serverAddress.value != "" && ws.State != WebSocketState.Connecting);
            registerButton.SetEnabled(playerName.value != "" && ws.State == WebSocketState.Open);
            spawnButton.SetEnabled(manager.registered && ws.State == WebSocketState.Open);
        }

        void Connect()
        {
            ws.Connect();
        }

        void ChangeAddress(ChangeEvent<string> e)
        {
            ws.ServerAddress = e.newValue;
        }

        void ChangePlayerName(ChangeEvent<string> e)
        {
            manager.playerName = e.newValue;
        }

        void Register()
        {
            manager.playerName = playerName.value;
            manager.Register();
        }

        void Spawn()
        {
            manager.SpawnSelf();
            root.style.display = DisplayStyle.None;
        }
    }
}