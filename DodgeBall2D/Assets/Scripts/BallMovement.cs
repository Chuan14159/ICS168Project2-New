using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BallMovement : NetworkBehaviour {

    private SpriteRenderer _spriteRenderer; // The Sprite Renderer component attached
    private Coroutine expireRoutine;        // The expiration routine
    [SyncVar(hook = "SetTeam")]
    private int team = -1;                  // The team that owns the ball

    // Returns team
    public int Team
    {
        get
        {
            return team;
        }
    }

    public override void OnStartClient ()
    {
        SetTeam(team);
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
    public void SetTeam (int value)
    {
        if (expireRoutine != null)
        {
            StopCoroutine(expireRoutine);
        }
        team = value;
        switch (team)
        {
            case -1:
                _spriteRenderer.color = Color.white;
                gameObject.layer = 0;
                break;
            case 0:
                _spriteRenderer.color = Color.red;
                gameObject.layer = 8;
                break;
            case 1:
                _spriteRenderer.color = Color.blue;
                gameObject.layer = 9;
                break;
        }
    }

    private void OnCollisionEnter2D (Collision2D collision)
    {
        if (!isServer)
            return;

        PlayerControl p = collision.collider.GetComponent<PlayerControl>();
        if (p != null)
        {
            Debug.Log(p.Invincible);
        }
        if (collision.collider.CompareTag("Player") && team != -1 && !p.Invincible)
        {
            if (p.Team == 0)
            {
                Scoreboard.instance.SetScore2(Scoreboard.instance.Score2 + 1);
            }
            else if (p.Team == 1)
            {
                Scoreboard.instance.SetScore1(Scoreboard.instance.Score1 + 1);
            }
        }
        else if (collision.collider.CompareTag("Ground"))
        {
            SetTeam(-1);
        }
    }

    // Start the expire coroutine
    public void StartExpire (float seconds)
    {
        expireRoutine = StartCoroutine(Expire(seconds));
    }

    // Set to neutral after given seconds
    private IEnumerator Expire (float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SetTeam(-1);
    }
}
