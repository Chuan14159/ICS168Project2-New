﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerControl : NetworkBehaviour {

    // Use this for initialization
    #region Attributes
    public float _speed;
    public float high;
    public float magnitude;

    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private PlayerTrigger _Trigger;
    private Animator _animator;

    public GameObject _pivot;
    public GameObject heldPos;
    public GameObject localPlayerIndicator;

    [SyncVar(hook = "AssignTeam")]
    private int team;
    [SyncVar(hook = "BeInvincible")]
    private bool invincible;
    private Coroutine flashRoutine;
    private float Horizontal;
    private GameObject spawnLocation;
    private List<GameObject> Feet;
    private bool[] doubleJump = new bool[] { false, true };
    private int doubleJumpIndex = 0;
    private GameObject held;
    private Vector3 heldPosition;
    //private bool FaceLeft;
    private bool isPickingBall;
    private bool jumping = false;
    #endregion

    #region Properties
    public int Team
    {
        get
        {
            return team;
        }
    }

    public bool Invincible
    {
        get
        {
            return invincible;
        }
    }
    #endregion

    #region Event Functions
    void Awake () {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _Trigger = GetComponentInChildren<PlayerTrigger>();
        isPickingBall = false;
        localPlayerIndicator.transform.localScale = gameObject.transform.localScale * 4;
        _animator = GetComponent<Animator>();
        //FaceLeft = false;
    }

    public override void OnStartLocalPlayer ()
    {
        base.OnStartLocalPlayer();
        localPlayerIndicator.SetActive(true);
        spawnLocation = GameObject.FindGameObjectWithTag("SpawnLocation");
        transform.position = spawnLocation.transform.position;
        Horizontal = spawnLocation.transform.position.x;
        Feet = new List<GameObject>();
        GiveChoice();
    }

    public override void OnStartClient ()
    {
        base.OnStartClient();
        AssignTeam(team);
        BeInvincible(invincible);
    }

    private void Update ()
    {
        if (!isLocalPlayer)
            return;

        /*Pick up Ball*/
        if (Input.GetKeyDown(KeyCode.F))
            PickUpball();

        /*holding Ball*/
        if (held != null)
            CmdHoldBall(held, heldPosition);

        if (Input.GetMouseButtonDown(0) && isPickingBall)
            ThrowBall();

        //jumping = Input.GetKeyDown(KeyCode.Space);
        localPlayerIndicator.transform.position = transform.position + Vector3.up/2;
    }

    // Update is called once per frame
    void FixedUpdate () {
        /*Local Player*/
        if (!isLocalPlayer)
            return;

        /*Jump*/
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        /*Player Movement*/
        Move();
        Aim();
        heldPosition = new Vector3(heldPos.transform.position.x, heldPos.transform.position.y);
        _animator.SetFloat("TurnLeft", Horizontal);
        //if (FaceLeft)
        //heldPosition = transform.position + new Vector3(-0.5f, 0);
        //else
        //heldPosition = transform.position + new Vector3(0.5f, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isLocalPlayer)
        {
            GameObject ground = collision.transform.gameObject;
            if (ground.tag == "Ground" || ground.tag == "Terrain")
            {
                ResetDoubleJump();
                if (!Feet.Contains(ground))
                    Feet.Add(ground);
            }
        }
        if (isServer)
        {
            BallMovement b = collision.collider.GetComponent<BallMovement>();
            if (b != null && b.Team != -1 && !invincible)
            {
                StartCoroutine(InvincibleTime(1f));
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        dividerAlpha divider = collision.transform.gameObject.GetComponent<dividerAlpha>();
        if (divider != null)
        {
            divider.show = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        dividerAlpha divider = collision.transform.gameObject.GetComponent<dividerAlpha>();
        if (divider != null)
        {
            divider.show = false;
        }

        if (!isLocalPlayer)
            return;

        GameObject ground = collision.transform.gameObject;
        if ((ground.tag == "Ground" || ground.tag == "Terrain") && Feet.Contains(ground))
        {
            Feet.Remove(ground);
            _animator.SetBool("isJumping", true);
        }
    }
    #endregion

    #region Methods
    private void Move()
    {
        Horizontal = Input.GetAxis("Horizontal") * _speed;
        _rigidbody.velocity = new Vector2(Horizontal, _rigidbody.velocity.y);
    }

    private void Aim()
    {
        Vector3 mouse_pos = Input.mousePosition;
        mouse_pos.z = -10f;
        Vector3 playerPos = Camera.main.WorldToScreenPoint(_pivot.transform.position);
        mouse_pos.x = mouse_pos.x - playerPos.x;
        mouse_pos.y = mouse_pos.y - playerPos.y;
        float angle = Mathf.Atan2(mouse_pos.x,mouse_pos.y) * Mathf.Rad2Deg;
        _pivot.transform.rotation = Quaternion.Euler(new Vector3(0,0,-angle));
    }
    private void Jump()
    {
        _animator.SetBool("isJumping", true);
        if (Feet.Count > 0 && doubleJumpIndex < doubleJump.Length && !doubleJump[doubleJumpIndex])
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, high);
            doubleJump[doubleJumpIndex++] = true;
        }
        else if(doubleJumpIndex < doubleJump.Length && doubleJump[doubleJumpIndex])
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, high);
            doubleJumpIndex++;
        }
    }

    private void ResetDoubleJump()
    {
        doubleJump[0] = false;
        doubleJump[1] = true;
        doubleJumpIndex = 0;
        _animator.SetBool("isJumping", false);
    }

    public void GiveChoice ()
    {
        GameController.instance.teamSelectionUI.SetActive(true);
        TeamSelection.instance.Player = this;
    }

    public void AssignTeam (int value)
    {
        team = value;
        gameObject.layer = value + 11;
        switch (value)
        {
            case 0:
                _spriteRenderer.color = new Color(1, 0.3f, 0);
                transform.position = new Vector3(-4, 4);
                break;
            case 1:
                _spriteRenderer.color = new Color(0, 0.6f, 1);
                transform.position = new Vector3(4, 4);
                break;
        }
    }

    private void BeInvincible (bool value)
    {
        invincible = value;
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            _spriteRenderer.color = _spriteRenderer.color.SetAlpha(1);
        }
        if (invincible)
        {
            flashRoutine = StartCoroutine(Flash());
        }
    }

    [Command]
    public void CmdChooseTeam (int value)
    {
        AssignTeam(value);
    }

    [Command]
    private void CmdHoldBall(GameObject g, Vector2 pos)
    {
        Rigidbody2D r = g.GetComponent<Rigidbody2D>();
        r.velocity = Vector2.zero;
        r.MovePosition(pos);
    }

    private void PickUpball()
    {
        isPickingBall = !isPickingBall;
        if (isPickingBall)
        {
            held = _Trigger.GetBall();
            if(held != null)
                held.GetComponent<Collider2D>().enabled = false;
            CmdPickUpBall(isPickingBall, held);
        }
        else
        {
            CmdPickUpBall(isPickingBall, held);
            if (held != null)
                held.GetComponent<Collider2D>().enabled = true;
            held = null;
        }
    }

    [Command]
    private void CmdPickUpBall(bool pickBall, GameObject g)
    {
        if (pickBall && g != null)
        {
            g.GetComponent<BallMovement>().PickUp(team);
        }
        else if (!pickBall && g != null)
        {
            g.GetComponent<BallMovement>().Drop();
        }
    }

    private void ThrowBall()
    {
        if (held == null)
            return;
        held.GetComponent<Collider2D>().enabled = true;
        Vector2 mouseInput = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouseInput - (Vector2)held.transform.position;
        direction.Normalize();
        CmdThrowBall(held, direction * magnitude);
        isPickingBall = !isPickingBall;
        held = null;
    }

    [Command]
    private void CmdThrowBall(GameObject g, Vector2 dir)
    {
        BallMovement b = g.GetComponent<BallMovement>();
        int m = b.GetMovementType();
        b.Drop();
        if (m == 0)
        {
            g.GetComponent<Rigidbody2D>().velocity = dir;
        }
        else if (m == 1)
        {
            b.moveSerpentine(dir);
        } 
        else if ( m == 2)
        {
            b.moveCharge(dir);
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator InvincibleTime(float time)
    {
        yield return null;
        BeInvincible(true);
        yield return new WaitForSeconds(time);
        BeInvincible(false);
    }

    private IEnumerator Flash()
    {
        while (true)
        {
            for (float t = 0f; t < 0.1f; t += Time.deltaTime)
            {
                _spriteRenderer.color = _spriteRenderer.color.SetAlpha(1f - (t / 0.1f));
                yield return null;
            }
            for (float t = 0f; t < 0.1f; t += Time.deltaTime)
            {
                _spriteRenderer.color = _spriteRenderer.color.SetAlpha(t / 0.1f);
                yield return null;
            }
        }
    }
    #endregion
}
