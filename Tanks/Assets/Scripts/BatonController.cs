using UnityEngine;
using System.Collections;

public class BatonController : MonoBehaviour {
    private Rigidbody2D rBody;
    private BoxCollider2D bCol;

    void Awake()
    {
        bCol = GetComponent<BoxCollider2D>();
        rBody = GetComponent<Rigidbody2D>();
    }

    public void Activate()
    {
        bCol.isTrigger = false;
        rBody.gravityScale = 1;
        transform.rotation = Quaternion.identity;
    }

    void OnCollisionEnter2D(Collision2D col) {
        rBody.velocity = Vector2.zero;
        bCol.isTrigger = true;
        rBody.gravityScale = 0;
        transform.rotation = Quaternion.identity;
    }
}
