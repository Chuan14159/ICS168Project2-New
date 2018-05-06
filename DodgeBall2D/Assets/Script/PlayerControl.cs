using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerControl : NetworkBehaviour {

    // Use this for initialization
    public float _speed;
    public float high;
    private readonly float Max_X = 6;
    private readonly float Min_X = -6;

    private float Horizontal;
    private GameObject spawnLocation;
    private List<GameObject> Feet;

	void Awake () {
        spawnLocation = GameObject.FindGameObjectWithTag("SpawnLocation");
        transform.position = spawnLocation.transform.position;
        Horizontal = spawnLocation.transform.position.x;
        Feet = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
        Move();
	}

    private void Move()
    {
        Horizontal += Input.GetAxis("Horizontal") * _speed * Time.deltaTime;
        Horizontal = Mathf.Clamp(Horizontal,Min_X,Max_X);

        Vector3 newPosition = new Vector3(Horizontal, transform.position.y, 0);
        GetComponent<Rigidbody>().MovePosition(newPosition);
    }

    private void Jump()
    {
        if(Feet.Count > 0)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * high,ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject ground = collision.transform.gameObject;
        if (ground.tag == "Ground" && !Feet.Contains(ground))
            Feet.Add(ground);
        Debug.Log("Enter" + Feet.Count);
    }

    private void OnCollisionExit(Collision collision)
    {
        GameObject ground = collision.transform.gameObject;
        if (ground.tag == "Ground" && Feet.Contains(ground))
            Feet.Remove(ground);
        Debug.Log("Exit" + Feet.Count);
    }
}
