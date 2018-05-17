using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportUp : MonoBehaviour {

    public float magnitute;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerStay2D(Collider2D other)
    {
        GameObject o = other.gameObject;
        if ( o.tag == "Player" || o.tag == "Ball" )
        {
            Rigidbody2D rb = o.GetComponent<Rigidbody2D>();

            rb.AddForce(Vector2.up * magnitute);
        }

    }
}
