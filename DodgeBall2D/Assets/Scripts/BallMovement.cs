using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BallMovement : NetworkBehaviour {

    private SpriteRenderer _spriteRenderer; // The Sprite Renderer component attached
    [SyncVar(hook = "SetAlive")]
    private bool alive = false;            // Whether the ball is in play or not

    public override void OnStartClient()
    {
        SetAlive(alive);
    }

    // Use this for initialization
    void Awake ()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
	}

    // Update is called once per frame
    void Update ()
    {
		
	}

    // Pick up the object
    public void SetAlive (bool value)
    {
        alive = value;
        _spriteRenderer.color = alive ? Color.yellow : Color.white;
    }

    private void OnCollisionEnter2D (Collision2D collision)
    {
        if (!isServer)
            return;

        if (collision.collider.CompareTag("Player") && alive)
        {
            if (collision.collider.GetComponent<PlayerControl>().Team == 0)
            {
                Scoreboard.instance.SetScore2(Scoreboard.instance.Score2 + 1);
            }
            else if (collision.collider.GetComponent<PlayerControl>().Team == 1)
            {
                Scoreboard.instance.SetScore1(Scoreboard.instance.Score1 + 1);
            }
        }
        else if (collision.collider.CompareTag("Ground"))
        {
            SetAlive(false);
        }
    }
}
