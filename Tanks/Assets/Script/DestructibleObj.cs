using UnityEngine;
using System.Collections;

public class DestructibleObj : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnCollisionEnter () {
	//change number to change amount of time before destroyed, to allow for
    //animations or whatever
        Destroy(gameObject, 0);
	}
}
