using UnityEngine;
using System.Collections;

public class RotateBullet : MonoBehaviour {
    public float rotationSpeed = 20;
    private Vector3 startRotation;
    private bool direction = false;//right
    // Use this for initialization
	void Start () {
        startRotation = transform.rotation.eulerAngles;
        direction = startRotation.z > 90;
        GetComponent<SpriteRenderer>().flipX = !direction;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        startRotation.z = startRotation.z + Time.deltaTime * rotationSpeed * (direction ? 1 : -1);
        transform.rotation = Quaternion.Euler(startRotation);
	}
}
