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
            float Min = 9999;
            foreach (GameObject g in Balls)
            {
                if (Min > (g.transform.position - gameObject.transform.position).magnitude)
                {
                    Min = (g.transform.position - gameObject.transform.position).magnitude;
                    PickUp = g;
                }
            }
            Debug.Log("Min" + Min);
        }
        else PickUp = null;
        Debug.Log("Pickup" + PickUp);
    }

    public GameObject GetBall()
    {
        GetClosestBall();
        return PickUp;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ball" && !Balls.Contains(collision.gameObject))
        {
            Balls.Add(collision.gameObject);
            Debug.Log("List B:" + Balls.Count);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ball" && Balls.Contains(collision.gameObject))
            Balls.Remove(collision.gameObject);
    }
}
