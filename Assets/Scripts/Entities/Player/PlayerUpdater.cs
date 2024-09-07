using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OutMessageData;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerAttack))]
public class PlayerUpdater : MonoBehaviour
{
    string id;
    Direction lastDir;
    Vector2 lastPosition;
    bool lastIsMoving;
    bool lastDashing;

    Player player;
    WebSockets ws;

    void Awake()
    {
        player = GetComponent<Player>();

        GameObject manager = GameObject.Find("Manager");
        ws = manager.GetComponent<WebSockets>();
        id = manager.GetComponent<GameManager>().id;

        lastDir = Direction.Down;
        lastPosition = transform.position;
        lastIsMoving = false;
        lastDashing = false;
    }

    void OnEnable()
    {
        GetComponent<PlayerAttack>().onAttack += sendAttack;
        GetComponent<PlayerAttack>().onHit += sendHit;
    }

    void OnDisable()
    {
        GetComponent<PlayerAttack>().onAttack -= sendAttack;
        GetComponent<PlayerAttack>().onHit -= sendHit;
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
        if (player.dir != lastDir)
        {
            lastDir = player.dir;
            change = true;
        }

        if (player.isMoving != lastIsMoving)
        {
            lastIsMoving = player.isMoving;
            change = true;
        }

        if (player.isDashing != lastDashing)
        {
            lastDashing = player.isDashing;
            change = true;
        }

        if (change) ws.SendWSMessage("update", new UpdateData
        {
            id = id,
            dir = player.dir,
            isMoving = player.isMoving,
            isDashing = player.isDashing
        });
    }

    public void sendAttack()
    {
        ws.SendWSMessage("attack", new AttackData { id = id });
    }

    public void sendHit(Entity entity)
    {
        ws.SendWSMessage("hit", new HitData { id = id, targetId = entity.netId });
    }
}
