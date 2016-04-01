using UnityEngine;

public class IsGrounded : MonoBehaviour {
    private bool grounded = false;
    
    public void Reset()
    {
        grounded = false;
    }

    void Update()
    {
        Collider2D[] hit = Physics2D.OverlapPointAll(transform.position);
        grounded = false;
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].tag == "Ground")
                grounded = true;
        }
    }

    public bool CheckGrounded()
    {
        return grounded;
    }
}
