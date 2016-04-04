using UnityEngine;
using System.Collections;

public class ExplodingKitten : MonoBehaviour {
    [HideInInspector]
    public GameObject player;
    public GameObject explosion;
    public float radius = 1f;
    private int playerID;
	// Use this for initialization
	void Start () {
        playerID = GetComponent<PlayerController>().GetPlayerID();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if((col.tag == "Player" && col.gameObject != player) || col.tag == "Ground")
        {
            //Explode
            Instantiate(explosion, transform.position, Quaternion.identity);
            //Physics
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
            foreach(Collider2D col1 in cols)
            {
                if (col1.tag == "Player" && col1.gameObject != player)
                    col1.GetComponent<PlayerController>().Attack(playerID);
            }
            //Destroy
            Destroy(gameObject);
        }
    }
}
