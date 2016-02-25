using UnityEngine;
using System.Collections;

public class DestructibleObj : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D col) {
        //change number to change amount of time before destroyed, to allow for
        //animations or whatever
        if (col.gameObject.tag == "Bullet")
        {
            Destroy(gameObject, 0);
        }
	}
}
