using UnityEngine;
using System.Collections;

public class SpearObject : MonoBehaviour, IPlayerID {
    private int playerID = -1;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        Destroy(gameObject);
    }

    public void SetPlayerID(int playerID)
    {
        this.playerID = playerID;
    }

    public int GetPlayerID()
    {
        return playerID;
    }
}
