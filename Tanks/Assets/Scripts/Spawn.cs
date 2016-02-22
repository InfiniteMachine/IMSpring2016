using UnityEngine;
using System.Collections.Generic;

public class Spawn : MonoBehaviour {

    private BoxCollider2D col;

    void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }

    public bool IsEmpty()
    {
        Collider2D[] cols = Physics2D.OverlapAreaAll(transform.position + col.bounds.center - col.bounds.extents, transform.position + col.bounds.center + col.bounds.extents);
        foreach(Collider2D obj in cols)
        {
            if (obj.gameObject.tag == "Player")
                return false;
        }
        return true;
    }
}
