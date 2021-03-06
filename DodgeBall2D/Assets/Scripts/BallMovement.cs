﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BallMovement : NetworkBehaviour {

    private SpriteRenderer _spriteRenderer; // The Sprite Renderer component attached
    private ParticleSystem _particleSystem; // The Particle System component used
    private Coroutine expireRoutine;        // The expiration routine
    [SyncVar(hook = "SetTeam")]
    private int team = -1;                  // The team that owns the ball
    [SyncVar(hook = "SetPickup")]
    private bool pickedUp = false;          // Whether the ball is picked up
    [SyncVar(hook = "SetMovementType")]
    private int movementType;
    public ParticleSystem[] particles = new ParticleSystem[2];

    // Returns team
    public int Team
    {
        get
        {
            return team;
        }
    }

    public override void OnStartServer ()
    {
        base.OnStartServer();
        SetMovementType(Random.Range(0, particles.Length));
    }

    public override void OnStartClient ()
    {
        base.OnStartClient();
        SetTeam(team);
        SetPickup(pickedUp);
        SetMovementType(movementType);
    }

    // Use this for initialization
    void Awake ()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _particleSystem = GetComponent<ParticleSystem>();
	}

    // Sets the type of movement that the ball has
    void SetMovementType (int value)
    {
        movementType = value;
        ParticleSystem.MainModule m = _particleSystem.main;
        m.startColor = particles[movementType].main.startColor;
    }

    public int GetMovementType(){
        return movementType;
    }

    public void moveSerpentine(Vector2 dir){
        transform.GetComponent<Rigidbody2D>().velocity = dir/2;
        StartCoroutine(serpentine());
        StartExpire(2);
    }

    public void moveCharge(Vector2 dir){
        StartCoroutine(charge(dir));
        StartExpire(3);
    }


    private IEnumerator serpentine(){
        for (int i = 0; i < 10; i++ )
        {
            yield return new WaitForSeconds(0.1f);
            transform.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 500.0f * Mathf.Pow(-1, i));
        }
    }


    private IEnumerator charge(Vector2 dir){
        for (int i = 10; i > 0; i--)
        {
            transform.GetComponent<Rigidbody2D>().velocity = dir / i;
            yield return new WaitForSeconds(0.07f);
        }
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

    // Set the pickup status of the object
    public void SetPickup (bool value)
    {
        pickedUp = value;
        if (pickedUp)
        {
            gameObject.layer = 13;
        }
        else
        {
            SetTeam(team);
        }
    }

    public void PickUp (int team)
    {
        SetTeam(team);
        SetPickup(true);
    }

    public void Drop ()
    {
        SetPickup(false);
        StartExpire(2);
    }

    private void OnCollisionEnter2D (Collision2D collision)
    {
        if (!isServer)
            return;

        PlayerControl p = collision.collider.GetComponent<PlayerControl>();
        if (p != null)
        {
            //Debug.Log(p.Invincible);
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
            //SetTeam(-1);
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
