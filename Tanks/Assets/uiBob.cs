using UnityEngine;
using System.Collections;

public class uiBob : MonoBehaviour {
    private Vector3 startPosition;
    public float offset;
    public float speed;
    private float counter;
	// Use this for initialization
	void Start () {
	    startPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        counter += Time.deltaTime * speed;
        transform.position = startPosition + transform.right * offset * Mathf.Abs(Mathf.Sin(counter));
	}
}
