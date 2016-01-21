using UnityEngine;

public class IsGrounded : MonoBehaviour {
    private Collider2D ground;
    private bool grounded = false;
    
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Ground")
        {
            ground = col;
            grounded = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col == ground)
        {
            grounded = false;
            ground = null;
        }
    }

    public bool CheckGrounded()
    {
        return grounded;
    }
}
