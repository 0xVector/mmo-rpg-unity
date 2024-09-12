# Technical Docs

## Architecture overview

The unity client is composed of multiple parts.

### Network

The network communication is a responsibility of the classes in the `WebSockets` namespace. The low-level connection and raw message sending and receiving related functionality is managed by the `WebSocketManager` class.  
The `GameManager` class provides event handlers for incoming messages and is responsible for processing them and updating the game state accordingly.  
The shape of the incoming and outgoing messages is defined by the `InMessageData` and `OutMessageData` classes.

An important part of the network stack is the `PlayerUpdater` class. It is responsible for sending the current player state and any new actions to the server via the `WebSocketManager`.
