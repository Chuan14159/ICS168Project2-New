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

    [SyncVar(hook = "AssignTeam")]
    private int team;
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
    #endregion

    #region Event Functions
    void Awake () {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _Trigger = GetComponentInChildren<PlayerTrigger>();
        //FaceLeft = false;
        isPickingBall = false;
    }

    public override void OnStartLocalPlayer ()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        spawnLocation = GameObject.FindGameObjectWithTag("SpawnLocation");
        transform.position = spawnLocation.transform.position;
        Horizontal = spawnLocation.transform.position.x;
        Feet = new List<GameObject>();
        GiveChoice();
    }

    public override void OnStartClient ()
    {
        AssignTeam(team);
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
        //if (FaceLeft)
            heldPosition = transform.position + new Vector3(-0.5f, 0);
        //else
            //heldPosition = transform.position + new Vector3(0.5f, 0);
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isLocalPlayer)
            return;

        GameObject ground = collision.transform.gameObject;
        if (ground.tag == "Ground" && !Feet.Contains(ground))
            Feet.Add(ground);
        ResetDoubleJump();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!isLocalPlayer)
            return;

        GameObject ground = collision.transform.gameObject;
        if (ground.tag == "Ground" && Feet.Contains(ground))
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
        gameObject.layer = value + 8;
        switch (value)
        {
            case 0:
                _spriteRenderer.color = Color.red;
                break;
            case 1:
                _spriteRenderer.color = Color.blue;
                break;
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
        g.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        g.GetComponent<Rigidbody2D>().MovePosition(pos);
    }

    private void PickUpball()
    {
        isPickingBall = !isPickingBall;
        if (isPickingBall)
        {
            held = _Trigger.GetBall();
        }
        else held = null;
        CmdPickUpBall(isPickingBall, held);
    }

    [Command]
    private void CmdPickUpBall(bool pickBall, GameObject g)
    {
        if (pickBall && g != null)
        {
            g.GetComponent<BallMovement>().SetAlive(true);
        }
    }

    private void ThrowBall()
    {
        Vector2 mouseInput = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouseInput - (Vector2)held.transform.position;
        direction.Normalize();
        isPickingBall = !isPickingBall;
        CmdThrowBall(held, direction * magnitude);
        held = null;
    }

    [Command]
    private void CmdThrowBall(GameObject g, Vector2 dir)
    {
        g.GetComponent<Rigidbody2D>().velocity = dir;
    }
    #endregion
}
