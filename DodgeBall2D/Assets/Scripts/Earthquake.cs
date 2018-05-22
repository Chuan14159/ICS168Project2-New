using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earthquake : MonoBehaviour {
    public GameObject timer;
    private int seconds;
    private int direction;
    private float tiltAngle;
    private float bound;
    public int finalstate;
	// Use this for initialization
	void Start () {
        direction = 1;
	}
	
	// Update is called once per frame
	void Update () {
        seconds = timer.gameObject.GetComponent<Timer>().seconds;
        Debug.Log(gameObject.transform.eulerAngles.z);
        if (seconds > 0 && seconds <= finalstate)
        {
            if(gameObject.transform.eulerAngles.z >= 1 && gameObject.transform.eulerAngles.z <= 180)
            {
                direction = -1;
            }
            if (gameObject.transform.eulerAngles.z <= 359 && gameObject.transform.eulerAngles.z >= 180)
            {
                direction = 1;
            }
            gameObject.transform.Rotate(new Vector3(0, 0, Random.Range(-0.1f,0.3f) * direction));
        }
	}
}
