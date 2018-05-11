using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour {


    private List<GameObject> Balls;
    private GameObject PickUp = null;

	// Use this for initialization
	void Awake () {
        Balls = new List<GameObject>();
	}

    private void GetClosestBall()
    {
        if (Balls.Count > 0)
        {
            float Min = 0;
            foreach (GameObject g in Balls)
            {
                if (Min > (g.transform.position - gameObject.transform.position).magnitude)
                {
                    Min = (g.transform.position - gameObject.transform.position).magnitude;
                    PickUp = g;
                }
            }
        }
        else PickUp = null;
    }

    public GameObject GetBall()
    {
        return PickUp;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ball" && !Balls.Contains(collision.gameObject))
            Balls.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ball" && Balls.Contains(collision.gameObject))
            Balls.Remove(collision.gameObject);
    }
}
