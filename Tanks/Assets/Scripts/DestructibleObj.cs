using UnityEngine;
using System.Collections;

public class DestructibleObj : MonoBehaviour {
    public int hits = 2;
    private int timesHit = 0;
    public Sprite[] sprites;
    private SpriteRenderer sRender;

    public GameObject pSystem;
    // Use this for initialization
	void Start () {
        sRender = GetComponent<SpriteRenderer>();
        sRender.sprite = sprites[0];
	}
	
	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D col) {
        //change number to change amount of time before destroyed, to allow for
        //animations or whatever
        if (col.gameObject.tag == "Bullet")
        {
            Hit();
        }
	}

    public void Hit()
    {
        if (pSystem != null)
            Instantiate(pSystem, transform.position, Quaternion.identity);
        timesHit++;
        if (timesHit >= hits)
            Destroy(gameObject, 0);
        else
            sRender.sprite = sprites[timesHit];
    }
}
