using UnityEngine;
using System.Collections;

public class ParrentCollision : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D col)
    {
        transform.parent.SendMessage("OnChildTriggerEnter2D", new object[] { col, name });
    }
}
