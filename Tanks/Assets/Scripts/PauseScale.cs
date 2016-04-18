using UnityEngine;
using System.Collections;

public class PauseScale : MonoBehaviour {
    private bool active = false;
    private Vector3 startScale;
    public float scaleSpeed = 5f;
    public float amplitude = 0.15f;
    private float counter = 0;
	// Use this for initialization
	void Start () {
        startScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
        if (active)
        {
            counter += Time.unscaledDeltaTime * scaleSpeed;
            transform.localScale = startScale + Vector3.one * amplitude * Mathf.Sin(counter);
         }
	}

    public void SetState(bool active)
    {
        this.active = active;
        if (!active)
        {
            counter = 0;
            transform.localScale = startScale;
        }
    }
}
