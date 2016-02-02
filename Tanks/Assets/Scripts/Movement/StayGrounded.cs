using UnityEngine;
using System.Collections;

public class StayGrounded : MonoBehaviour {

    private bool stayDown = true;
    public float distance = 3f;
    [Range(0f, 1f)]
    public float errorPercentage = 0.8f;
    private Rigidbody2D rBody;
    private CircleCollider2D colider;
    
    private float timer = 0;
    // Use this for initialization
    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        colider = GetComponent<CircleCollider2D>();
    }
    
    void Update()
    {
        if (timer >= 0)
            timer -= Time.deltaTime;
        else
            stayDown = true;
    }

    public void Ignore(float duration)
    {
        timer = duration;
        stayDown = false;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            if (stayDown)
            {
                if (col.transform.position.y < transform.position.y)
                {
                    RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + colider.offset, Vector2.down, distance);
                    if (hit)
                    {
                        Vector2 normal = -hit.normal;
                        RaycastHit2D hit2 = Physics2D.Raycast((Vector2)transform.position + colider.offset, normal, distance);
                        if (hit2)
                        {
                            Vector2 vec = hit2.point - ((Vector2)transform.position + colider.offset);
                            vec = vec.normalized * (vec.magnitude - colider.radius) * errorPercentage;
                            vec.x = 0;
                            rBody.MovePosition(transform.position + (Vector3)vec);
                        }
                        rBody.velocity = new Vector2(rBody.velocity.x, Mathf.Min(-5f, rBody.velocity.y));
                    }
                }
            }
        }
    }
}