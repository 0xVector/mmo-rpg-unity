using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OutMessageData;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerAttack))]
public class PlayerUpdater : MonoBehaviour
{
    Player player;
    WebSockets ws;
    string id;
    Direction lastFacing;
    Vector2 lastPosition;
    bool lastIsRunning;

    void Awake()
    {
        player = GetComponent<Player>();
        
        GameObject manager = GameObject.Find("Manager");
        ws = manager.GetComponent<WebSockets>();
        id = manager.GetComponent<GameManager>().id;

        lastFacing = Direction.Down;
        lastPosition = transform.position;
        lastIsRunning = false;
    }

    void OnEnable()
    {
        GetComponent<PlayerAttack>().onAttack += sendAttack;
    }

    void OnDisable()
    {
        GetComponent<PlayerAttack>().onAttack -= sendAttack;
    }

    void Update()
    {
        updatePosition();
        updateState();
    }

    void updatePosition()
    {
        Vector2 pos = transform.position;
        if (pos != lastPosition)
        {
            lastPosition = pos;
            ws.SendWSMessage("move", new MoveData { id = id, x = pos.x, y = pos.y });
        }
    }

    void updateState()
    {
        bool change = false;
        if (player.dir != lastFacing)
        {
            lastFacing = player.dir;
            change = true;
        }

        if (player.isRunning != lastIsRunning)
        {
            lastIsRunning = player.isRunning;
            change = true;
        }

        if (change) ws.SendWSMessage("update", new UpdateData
        {
            id = id,
            facing = player.dir,
            isRunning = player.isRunning,
            isAttacking = false
        });
    }

    public void sendAttack()
    {
        ws.SendWSMessage("update", new UpdateData
        {
            id = id,
            facing = player.dir,
            isRunning = player.isRunning,
            isAttacking = true
        });
    }
}
