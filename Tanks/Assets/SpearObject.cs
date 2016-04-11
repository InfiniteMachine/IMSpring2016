using UnityEngine;
using System.Collections;

public class SpearObject : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        Destroy(gameObject, 15f);
	}
	
	// Update is called once per frame
	void Update () {
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        Destroy(gameObject);
    }
}
