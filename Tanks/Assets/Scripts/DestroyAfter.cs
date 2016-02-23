using UnityEngine;
using System.Collections;

public class DestroyAfter : MonoBehaviour {
    public float aliveDuration = 5f;
	// Use this for initialization
	void Start () {
        Destroy(gameObject, aliveDuration);
	}
}
