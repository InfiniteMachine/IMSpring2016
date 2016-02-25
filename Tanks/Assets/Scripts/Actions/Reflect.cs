using UnityEngine;
using System.Collections;

public class Reflect : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Bullet")
        {
            Rigidbody2D rBody = col.gameObject.GetComponent<Rigidbody2D>();
            RaycastHit2D hit = Physics2D.Raycast(col.transform.position, rBody.velocity.normalized);
            if (hit)
            {
                if (hit.collider.gameObject == gameObject)
                    rBody.velocity = Vector2.Reflect(rBody.velocity, hit.normal);
                else
                    rBody.velocity = -rBody.velocity;
            }
            else
                rBody.velocity = -rBody.velocity;
        }
    }
}
