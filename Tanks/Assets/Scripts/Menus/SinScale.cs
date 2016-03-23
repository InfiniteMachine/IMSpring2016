using UnityEngine;
using System.Collections;

public class SinScale : MonoBehaviour {
    public float amplitude = 0.25f;
    public float speed = 2f;
    private float counter = 0;
    private Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
    }

	// Update is called once per frame
	void Update () {
        counter += Time.deltaTime * speed;
        transform.localScale = startScale + (Vector3.one * amplitude * Mathf.Sin(counter));
	}
}
