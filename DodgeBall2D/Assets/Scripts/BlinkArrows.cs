using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkArrows : MonoBehaviour {

    public string color;

    private List<Sprite> colorSet;

    public List<Sprite> blue = new List<Sprite>();
    public List<Sprite> orange = new List<Sprite>();
    public List<Sprite> purple = new List<Sprite>();

    private List<GameObject> arrows;

	// Use this for initialization
	void Start () {
        if (color == "orange")
            colorSet = orange;
        else if (color == "purple")
            colorSet = purple;
        else
            colorSet = blue;
        
        arrows = new List<GameObject>();
        foreach (Transform child in transform)
        {
            arrows.Add(child.gameObject);
            child.GetComponent<SpriteRenderer>().sprite = colorSet[0];
        }
        StartCoroutine( blink() );	
	}

    private IEnumerator blink(){
        WaitForSeconds waitTime = new WaitForSeconds(0.25f);
        WaitForSeconds resetTime = new WaitForSeconds(0.5f);
        while (true){
            yield return resetTime;
            foreach (GameObject arrow in arrows){
                
                arrow.GetComponent<SpriteRenderer>().sprite = colorSet[1];
                yield return waitTime;
            }
            yield return resetTime;
            foreach (GameObject arrow in arrows)
            {
                arrow.GetComponent<SpriteRenderer>().sprite = colorSet[0];
            }
        }
    }
}
