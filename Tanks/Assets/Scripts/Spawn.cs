using UnityEngine;
using System.Collections.Generic;

public class Spawn : MonoBehaviour {

    private BoxCollider2D col;

    private List<Collider2D> playerColliders;

    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        playerColliders = new List<Collider2D>();
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in gos)
            playerColliders.Add(go.GetComponent<BoxCollider2D>());
    }

    public bool IsEmpty()
    {
        foreach (Collider2D obj in playerColliders)
        {
            if(col.IsTouching(obj))
                return false;
        }
        return true;
    }
}
