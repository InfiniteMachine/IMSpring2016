using UnityEngine;
using System.Collections;

public class BatonController : MonoBehaviour {
    private Rigidbody2D rBody;
    private BoxCollider2D bCol;
    private float bottom;
    public float ignoreDuration = 0.1f;
    void Awake()
    {
        bCol = GetComponent<BoxCollider2D>();
        rBody = GetComponent<Rigidbody2D>();
        bottom = Camera.main.GetComponent<CamFollow>().GetLowerBound();
    }

    public void Update()
    {
        if (transform.position.y < bottom)
        {
            Manager.instance.ResetBaton(true);
            rBody.velocity = Vector2.zero;
        }
    }

    public void Activate()
    {
        bCol.isTrigger = false;
        rBody.gravityScale = 1;
        transform.rotation = Quaternion.identity;
        rBody.velocity = Vector2.zero;
        StartCoroutine(IgnoreCoroutine());
    }

    private IEnumerator IgnoreCoroutine()
    {
        Collider2D[] cols = Physics2D.OverlapAreaAll((Vector2)transform.position - bCol.size / 2, (Vector2)transform.position + bCol.size / 2);
        for(int i = 0; i < cols.Length; i++)
        {
            Physics2D.IgnoreCollision(cols[i], bCol);
        }
        float counter = 0;
        while (counter < ignoreDuration && bCol.enabled)
        {
            yield return null;
                counter += Time.deltaTime;
        }
        if (bCol.enabled)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                Physics2D.IgnoreCollision(cols[i], bCol, false);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        rBody.velocity = Vector2.zero;
        bCol.isTrigger = true;
        rBody.gravityScale = 0;
        transform.rotation = Quaternion.identity;
    }
}
