using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerControl : NetworkBehaviour {

    // Use this for initialization
#region Attributes
    public float _speed;
    public float high;

    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;

    [SyncVar(hook = "OnTeamChange")]
    private int team = -1;
    private float Horizontal;
    private GameObject spawnLocation;
    private List<GameObject> Feet;
    private bool[] doubleJump = new bool[] { false, true };
    private int doubleJumpIndex = 0;
#endregion

#region Event Functions
    void Awake () {
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
        AssignColor();
    }

    // Update is called once per frame
    void FixedUpdate () {
        Debug.Log(team);
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
        Move();
        Debug.Log(Feet.Count);
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isLocalPlayer)
            return;

        GameObject ground = collision.transform.gameObject;
        if (ground.tag == "Ground" && !Feet.Contains(ground))
            Feet.Add(ground);
        ResetDoubleJump();
        Debug.Log("Enter" + Feet.Count);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!isLocalPlayer)
            return;

        GameObject ground = collision.transform.gameObject;
        if (ground.tag == "Ground" && Feet.Contains(ground))
            Feet.Remove(ground);
        Debug.Log("Exit" + Feet.Count);
    }
#endregion

#region Methods
    private void Move()
    {
        Horizontal = Input.GetAxis("Horizontal") * _speed;
        _rigidbody.velocity = new Vector2(Horizontal, _rigidbody.velocity.y);
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

    public void AssignColor ()
    {
        gameObject.layer = team + 8;
        switch (team)
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

    public void OnTeamChange (int value)
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
#endregion
}
