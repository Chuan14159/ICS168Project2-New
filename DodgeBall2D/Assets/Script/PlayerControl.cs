using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerControl : NetworkBehaviour {

    // Use this for initialization
    public float _speed;

    private float Vertical;
    private float Horizontal;
    private GameObject spawnLocation; 

	void Awake () {
        spawnLocation = GameObject.FindGameObjectWithTag("SpawnLocation");
        transform.position = spawnLocation.transform.position;
        Vertical = spawnLocation.transform.position.y;
        Horizontal = spawnLocation.transform.position.x;
    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
            return;
        Move();
	}

    private void Move()
    {
        Vertical += Input.GetAxis("Vertical") * _speed * Time.deltaTime;
        Horizontal += Input.GetAxis("Horizontal") * _speed * Time.deltaTime;

        Vector3 newPosition = new Vector3(Horizontal, Vertical, 0);
        GetComponent<Rigidbody>().MovePosition(newPosition);
    }
}
