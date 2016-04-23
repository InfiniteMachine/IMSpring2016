using UnityEngine;

public class IsGrounded : MonoBehaviour {
    private bool reset = false;
    private Vector2 pointA, pointB;

    public void Start()
    {
        reset = false;
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        pointA = col.offset - (Vector2)col.bounds.extents;
        pointB = col.offset + (Vector2)col.bounds.extents;
    }

    public void Reset()
    {
        if (CheckGrounded())
            reset = true;
    }

    public bool CheckGrounded()
    {
        if (reset)
            return false;
        Collider2D[] cols = Physics2D.OverlapAreaAll((Vector2)transform.position + pointA, (Vector2)transform.position + pointB);

        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].tag == "Ground")
                return true;
        }
        return false;
    }
    
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Ground")
        {
            if (reset)
                reset = false;
        }
    }
}
