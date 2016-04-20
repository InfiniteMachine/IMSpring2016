using UnityEngine;
using System.Collections;

public class KeepStraight : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
        transform.rotation = Quaternion.identity;
	}
}
