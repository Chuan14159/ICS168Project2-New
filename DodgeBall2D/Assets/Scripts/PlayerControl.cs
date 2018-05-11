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
    private int team = -1;
    private float Horizontal;
    private GameObject spawnLocation;
    private List<GameObject> Feet;
    private bool[] doubleJump = new bool[] { false, true };
    private int doubleJumpIndex = 0;
    private GameObject held;
    private Vector3 heldPosition;
    private bool FaceLeft;
    private bool isPickingBall;
#endregion

#region Event Functions
    void Awake () {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _Trigger = GetComponentInChildren<PlayerTrigger>();
        FaceLeft = false;
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

    // Update is called once per frame
    void FixedUpdate () {
        /*Local Player*/
        if (!isLocalPlayer)
            return;

        /*Pick up Ball*/
        if (Input.GetKeyDown(KeyCode.F))
            PickUpBall();

        /*holding Ball*/
        if (held != null)
            HoldBall();

        /*Jump*/
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (Input.GetMouseButtonDown(0) && isPickingBall)
            held = ThrowBall();

        /*Player Movement*/
        Move();
        if (FaceLeft)
            heldPosition = transform.position + new Vector3(-0.5f, 0);
        else
            heldPosition = transform.position + new Vector3(0.5f, 0);
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
        if (Horizontal > 0)
            FaceLeft = false;
        if (Horizontal < 0)
            FaceLeft = true;
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

    private void HoldBall()
    {
        held.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        held.transform.position = heldPosition;
    }

    private void PickUpBall()
    {
        isPickingBall = !isPickingBall;
        if (isPickingBall)
            held = _Trigger.GetBall();
        else held = null;
    }
    private GameObject ThrowBall()
    {
         Vector2 MouseInput = Camera.main.ScreenToWorldPoint(Input.mousePosition);
         Vector2 direction = MouseInput - (Vector2)held.transform.position;
         direction.Normalize();
         held.GetComponent<Rigidbody2D>().velocity = direction * magnitude;
         isPickingBall = !isPickingBall;
         return null;
    }
#endregion
}
