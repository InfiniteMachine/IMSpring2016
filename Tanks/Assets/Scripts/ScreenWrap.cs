using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScreenWrap : MonoBehaviour {
    private BoxCollider2D left;
    private BoxCollider2D right;
    public float resetTime = 0.5f;

    private List<PlayerController> player;

    void Start()
    {
        left = transform.FindChild("LeftSide").GetComponent<BoxCollider2D>();
        right = transform.FindChild("RightSide").GetComponent<BoxCollider2D>();
        player = new List<PlayerController>();
    }

    void Update()
    {
        for (int i = 0; i < player.Count; i++)
        {
            if (!player[i].Intersects(left) && !player[i].Intersects(right))
            {
                player[i].IgnoreCollision(right, false);
                player[i].IgnoreCollision(left, false);
                player.RemoveAt(i);
                i--;
            }
        }
    }

    void OnChildTriggerEnter2D(object[] data)
    {
        Collider2D col = (Collider2D)data[0];
        string name = (string)data[1];
        if (col.tag == "Bullet")
        {
            Physics2D.IgnoreCollision(right, col);
            Physics2D.IgnoreCollision(left, col);
            if (name.Contains("Left"))
                col.gameObject.transform.position = right.transform.position;
            else
                col.gameObject.transform.position = left.transform.position;
            Rigidbody2D rBody = col.GetComponent<Rigidbody2D>();
            rBody.velocity = new Vector2(rBody.velocity.x, 0);
        }
        else if (col.tag == "Player")
        {
            PlayerController pCont = col.GetComponent<PlayerController>();
            if (!player.Contains(pCont))
            {
                if (name.Contains("Left"))
                    col.gameObject.transform.position = right.transform.position;
                else
                    col.gameObject.transform.position = left.transform.position;
                pCont.IgnoreCollision(left);
                pCont.IgnoreCollision(right);
                player.Add(pCont);
                Rigidbody2D rBody = col.GetComponent<Rigidbody2D>();
                rBody.velocity = new Vector2(rBody.velocity.x, 0);
            }
        }
    }
}