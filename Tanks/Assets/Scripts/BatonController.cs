using UnityEngine;

public class BatonController : MonoBehaviour {
    private Rigidbody2D rBody;
    private BoxCollider2D bCol;
    private float bottom;
    void Awake()
    {
        bCol = GetComponent<BoxCollider2D>();
        rBody = GetComponent<Rigidbody2D>();
        bottom = Camera.main.GetComponent<CamFollow>().GetLowerBound();
    }

    public void Update()
    {
        if(transform.position.y < bottom)
            Manager.instance.ResetBaton(true);
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
