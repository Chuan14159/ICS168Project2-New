using System.Collections;
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

    public GameObject _pivot;
    public GameObject heldPos;

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
        //FaceLeft = false;
    }

    public override void OnStartLocalPlayer ()
    {
        base.OnStartLocalPlayer();
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

        jumping = Input.GetKeyDown(KeyCode.Space);
    }

    // Update is called once per frame
    void FixedUpdate () {
        /*Local Player*/
        if (!isLocalPlayer)
            return;

        /*Jump*/
        if (jumping)
            Jump();

        /*Player Movement*/
        Move();
        Aim();
        heldPosition = new Vector3(heldPos.transform.position.x, heldPos.transform.position.y);
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
            if ((ground.tag == "Ground" || ground.tag == "Terrain") && !Feet.Contains(ground))
                Feet.Add(ground);
            ResetDoubleJump();
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

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!isLocalPlayer)
            return;

        GameObject ground = collision.transform.gameObject;
        if ((ground.tag == "Ground" || ground.tag == "Terrain") && Feet.Contains(ground))
            Feet.Remove(ground);
    }
    #endregion

    #region Methods
    private void Move()
    {
        Horizontal = Input.GetAxis("Horizontal") * _speed;
        _rigidbody.velocity = new Vector2(Horizontal, _rigidbody.velocity.y);
        /*if (Horizontal > 0)
            FaceLeft = false;
        if (Horizontal < 0)
            FaceLeft = true;*/
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
                _spriteRenderer.color = Color.red;
                transform.position = new Vector3(-4, 4);
                break;
            case 1:
                _spriteRenderer.color = Color.blue;
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
        g.GetComponent<BallMovement>().SetTeam(team);
    }

    private void PickUpball()
    {
        isPickingBall = !isPickingBall;
        if (isPickingBall)
        {
            held = _Trigger.GetBall();
            CmdPickUpBall(isPickingBall, held);
        }
        else
        {
            CmdPickUpBall(isPickingBall, held);
            held = null;
        }
    }

    [Command]
    private void CmdPickUpBall(bool pickBall, GameObject g)
    {
        if (pickBall && g != null)
        {
            g.GetComponent<BallMovement>().SetTeam(team);
        }
        else if (!pickBall && g != null)
        {
            g.GetComponent<BallMovement>().SetTeam(-1);
        }
    }

    private void ThrowBall()
    {
        if (held == null)
            return;
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
        int m = g.GetComponent<BallMovement>().GetMovementType();
        if (m == 0)
        {
            g.GetComponent<Rigidbody2D>().velocity = dir;
            g.GetComponent<BallMovement>().StartExpire(2);
        }
        else if (m == 1)
        {
            g.GetComponent<BallMovement>().moveSerpentine(dir);
        } 
        else if ( m == 2)
        {
            g.GetComponent<BallMovement>().moveCharge(dir);
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
