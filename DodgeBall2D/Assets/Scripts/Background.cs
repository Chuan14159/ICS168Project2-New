using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {
    public float initX;
    private float transX;
	// Use this for initialization
	void Start () {
        gameObject.transform.position = new Vector3(initX,0,1);
        transX = initX;
	}
	
	// Update is called once per frame
	void Update () {
        transX += 5 * Time.deltaTime;
        if (transX > 14)
        {
            transX = -14;
        }
		gameObject.transform.position = new Vector3(transX, 0, 1);
    }
}
