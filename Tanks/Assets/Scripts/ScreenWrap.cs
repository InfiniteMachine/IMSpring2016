using UnityEngine;
using System.Collections;

public class ScreenWrap : MonoBehaviour {
    private BoxCollider2D left;
    private BoxCollider2D right;
    public float resetTime = 0.5f;
    
    void Start()
    {
        left = transform.FindChild("LeftSide").GetComponent<BoxCollider2D>();
        right = transform.FindChild("RightSide").GetComponent<BoxCollider2D>();
    }

    void OnChildTriggerEnter2D(object[] data)
    {
        Collider2D col = (Collider2D)data[0];
        string name = (string)data[1];
        if (col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Bullet"))
        {
            Physics2D.IgnoreCollision(right, col);
            Physics2D.IgnoreCollision(left, col);
            if (name.Contains("Left"))
                col.gameObject.transform.position = right.transform.position;
            else
                col.gameObject.transform.position = left.transform.position;
            StartCoroutine(Reset(col, resetTime));
        }
    }

    IEnumerator Reset(Collider2D col1, float delay)
    {
        yield return new WaitForSeconds(delay);
        Physics2D.IgnoreCollision(col1, left, false);
        Physics2D.IgnoreCollision(col1, right, false);
    }
}