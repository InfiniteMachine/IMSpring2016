using UnityEngine;
using System.Collections;

public class Freezer : MonoBehaviour {
    [HideInInspector]
    public GameObject player;
    public float duration = 3f;
    private PlayerController pCont;
    public int hits = 2;
    private int timesHit = 0;
    // Use this for initialization
	void Start () {
        pCont = player.GetComponent<PlayerController>();
        pCont.Freeze(duration);
        Destroy(gameObject, duration);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Bullet")
        {
            timesHit++;
            if (timesHit >= hits)
            {
                pCont.Attack();
                Destroy(gameObject);
            }
            Destroy(col.gameObject);
        }
    }
}