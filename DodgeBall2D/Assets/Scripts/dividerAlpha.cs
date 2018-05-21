using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dividerAlpha : MonoBehaviour {
    public bool show;
    private float alpha;
	// Use this for initialization
	void Start () {
        show = false;
        alpha = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (show == true)
        {
            if (alpha <= 0.1)
            {
                alpha += 2 * Time.deltaTime;
            }
            else if (alpha <= 0.5)
            {
                alpha += 0.5f * Time.deltaTime;
            }
            transform.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, alpha);
        }
        else if (alpha >= 0)
        {
            alpha -= 3 * Time.deltaTime;
            transform.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, alpha);
        }
	}
}
