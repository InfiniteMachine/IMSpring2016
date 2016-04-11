using UnityEngine;
using System.Collections;

public class ExplodingKitten : MonoBehaviour, IPlayerID {
    [HideInInspector]
    public GameObject player;
    public GameObject explosion;
    public float radius = 1f;
    private int playerID;
    public void SetPlayerID(int id) { playerID = id; }
    public int GetPlayerID() { return playerID; }
    // Update is called once per frame
    void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if ((col.tag == "Player" && col.gameObject != player) || col.tag == "Ground")
        {
            //Explode
            SoundManager.instance.PlayOneShot("Explosion");
            Instantiate(explosion, transform.position, Quaternion.identity);
            //Physics
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
            foreach (Collider2D col1 in cols)
            {
                if (col1.tag == "Player" && col1.gameObject != player)
                {
                    PlayerController pCont = col1.gameObject.GetComponent<PlayerController>();
                    if (!pCont.IsShield())
                        pCont.Attack(playerID);
                }
                else if (col1.tag == "Ground")
                {
                    DestructibleObj des = col1.GetComponent<DestructibleObj>();
                    if (des != null)
                    {
                        des.Hit();
                    }
                }
                //Destroy
                Destroy(gameObject);
            }
        }
    }
}
