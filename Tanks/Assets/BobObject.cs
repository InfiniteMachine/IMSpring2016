using UnityEngine;
using System.Collections;

public class BobObject : MonoBehaviour {

    public float amplitude = 0.5f;
    private float min = 2f;
    private float max = 3f;

    // Use this for initialization
    void Start()
    {
        min = transform.localPosition.y;
        max = transform.localPosition.y + amplitude;
    }
	// Update is called once per frame
	void Update () {
        transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Abs(Mathf.Sin(Time.time*1*Mathf.PI))*(max- min)+min, transform.localPosition.z);
    }
}
