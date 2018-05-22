using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkArrows : MonoBehaviour {

    public Sprite arrowOff;
    public Sprite arrowOn;

    private List<GameObject> arrows;

	// Use this for initialization
	void Start () {
        arrows = new List<GameObject>();
        foreach (Transform child in transform)
        {
            arrows.Add(child.gameObject);
        }
        StartCoroutine( blink() );	
	}

    private IEnumerator blink(){
        WaitForSeconds waitTime = new WaitForSeconds(0.25f);
        WaitForSeconds resetTime = new WaitForSeconds(0.5f);
        while (true){
            yield return resetTime;
            foreach (GameObject arrow in arrows){
                
                arrow.GetComponent<SpriteRenderer>().sprite = arrowOn;
                yield return waitTime;
            }
            yield return resetTime;
            foreach (GameObject arrow in arrows)
            {
                arrow.GetComponent<SpriteRenderer>().sprite = arrowOff;
            }
        }
    }
}
