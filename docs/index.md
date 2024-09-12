# Technical Docs

## Architecture overview

The unity client is composed of multiple parts.

### Network

#### Overview

The network communication is a responsibility of the classes in the `WebSockets` namespace. The low-level connection and raw message sending and receiving related functionality is managed by the `WebSocketManager` class.  
The `GameManager` class provides event handlers for incoming messages and is responsible for processing them and updating the game state accordingly.  
The shape of the incoming and outgoing messages is defined by the `InMessageData` and `OutMessageData` classes.

An important part of the network stack is the `PlayerUpdater` class. It is responsible for sending the current player state and any new actions to the server via the `WebSocketManager`.

#### Inbound message flow

An inbound message is received by the `WebSocketManager`. It is deserialized (from JSON) into a `WebSocketMessage` object that exposes the two key high-level fields: `event` and `data`. The string `event` field is used to determine which event handler from the `GameManager` is responsible for processing this event message. The `data` field has to be later deserialized into a specific message type based on the `event` field. For now, it is kept as a *dynamic* object and is upon calling the appropriate event handler, it is serialized back to a JSON string. This is a workaround to the dynamic nature of the incoming messages, which can have different shapes based on the `event` field and is planned to be replaced with a more robust solution in the future.

The `GameManager` event handler then deserializes the `data` field into a specific message type and processes it accordingly. These message types are defined by dataclasses in the `WebSockets.InMessageData` namespace.

### Game

#### Prefabs

The game itself is based on the traditional Unity compnent-based architecture. The two types of live creatures in the game are the player and the slimes. These are both constructed from prefabs.

##### Player

The *Player* prefab acts as a base character that is controllable by the server via network messages. It can't or take any actions on its own, but is fully equipped with all the animations, physics and sprites.  
It is composed of these scripts:
- `Health` script manages the health of the player (or any other GameObject)
- `Player` script which contains the rest of the player-specific logic

To make the player controllable, the *PlayerControllable* prefab variant exists. This is the prefab used for the player controlled by the client. It has several overrides on the base *Player*, mostly added capabilities:
- `PlayerAttack` script, which reads the user input and allows the on-screen player attack
- `Movement` script, which reacts to the user input and makes the player move
- `PlayerUpdater` script, which sends the new state and actions to the server

##### Slime

The *Slime* prefab is a fully functional, server-controlled enemy. Like the basic *Player* prefab, it has all the animations, physics and sprites, but does no actions on its own. The `GameManager` class is responsible for creating and updating the slimes with new positions and actions from the server. It has a *SlimePurple* variant, which is a stronger version but functionally identical.  
Both are composed of these scripts:
- `Health` script is the same as for the player and manages the health of the slime
- `Slime` script which contains the rest of the slime-specific logic

#### Scripts

In designing the game, I made the effort to make the scripts mentioned above as modular as possible. This means that some of the scripts are fully independent of the GameObject they are attached to or at least only depend on a specific interface or higher-level class.  
A good example of this is the `Health` script, which can be attached to any GameObject and will manage its health. It is used by both the player and the slimes. It fulfills the `IDamageable` interface, which is used by the `GameManager` to process `damage` messages from the server. This has the advantages that, for example, to implement a new type of health system, or an object that can just take damage, it's only necessary to implement the `IDamageable` interface.
