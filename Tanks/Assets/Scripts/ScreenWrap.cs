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
        if (col.gameObject.CompareTag("Player"))
        {
            if (name.Contains("Left"))
            {
                //Going Left
                Physics2D.IgnoreCollision(right, col);
                col.gameObject.transform.position = right.transform.position;
                StartCoroutine(Reset(col, right, resetTime));
            }
            else
            {
                //Going Right
                Physics2D.IgnoreCollision(left, col);
                col.gameObject.transform.position = left.transform.position;
                StartCoroutine(Reset(col, left, resetTime));
            }
        }
    }

    IEnumerator Reset(Collider2D col1, Collider2D col2, float delay)
    {
        yield return new WaitForSeconds(delay);
        Physics2D.IgnoreCollision(col1, col2, false);
    }
}