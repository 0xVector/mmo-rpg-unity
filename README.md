# mmo-rpg-unity

#### An Unity client for the **[mmo-rpg](https://github.com/0xVector/mmo-rpg)** game server.

[*See the full docs*](0xvector.me/mmo-rpg-unity/)

## Features

### Technical

The client communicates with by the server by establishing a WebSocket connection.  
The server address to connect to is user selectable in a menu.
The client and server communicate using a custom protocol based on JSON messages. They engage in a basic keep-alive mechanism to ensure inactive clients are disconnected.

The game also has an older "native" [web client](https://github.com/0xVector/mmo-rpg) made in JS. The clients are fully compatible and can play together, even though the Unity client is of higher quality (thanks to better engine) and has some extra features.

### Game

It is a 2D top-down game inspired by pixel art RPGs. The player can move around the map and attack enemies. Enemies are spawned by the server and move around the map, attacking nearby players. Player gains score by killing enemies. The player can also interact with other players in the game.

#### Controls

Use `WASD` or Arrow keys to move around. Press `<Space>` or `<Left Mouse Button>` to attack. Hold `<Shift>` to dash.

#### Stats

The player has `10 HP`. There are 2 kinds of enemies: Slime and Purple Slime. They have `2 HP` and `4 HP` respectively. The player deals `1 damage point` per attack.
The slimes deal `1` and `2 damage points` respectively. For attacks (both player and enemy) to hit, the attack animation must actually play out and hit the target.

## Installation

The client is built in Unity `2022.3` targetting WebGL. It can be played in a browser.  
To play, you need the server. Get the server from [here](https://github.com/0xVector/mmo-rpg/tree/main/server).
