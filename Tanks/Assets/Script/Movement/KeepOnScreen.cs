using UnityEngine;
using System.Collections;

public class KeepOnScreen : MonoBehaviour {
    //Screen & Movement
    public float yComponent;
    public float xComponent;

    public float x, y;
    public Vector3 position;
    void Start () {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        //Calculate Screen Space of Collider
        Vector2 worldPoint1 = transform.position;
        worldPoint1.y += col.bounds.size.y / 2;
        Vector2 worldPoint2 = transform.position;
        worldPoint2.y -= col.bounds.size.y / 2;
        yComponent = Mathf.Abs(Camera.main.WorldToViewportPoint(worldPoint1).y - Camera.main.WorldToViewportPoint(worldPoint2).y);
        worldPoint1 = transform.position;
        worldPoint1.x += col.bounds.size.x / 2;
        worldPoint2 = transform.position;
        worldPoint2.x -= col.bounds.size.x / 2;
        xComponent = Mathf.Abs(Camera.main.WorldToViewportPoint(worldPoint1).x - Camera.main.WorldToViewportPoint(worldPoint2).x);
        Destroy(col);
    }

    void LateUpdate()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.parent.position);
        pos.x = Mathf.Clamp(pos.x, xComponent / 2, 1 - (xComponent / 2));
        pos.y = Mathf.Clamp(pos.y, yComponent / 2, 1 - (yComponent / 2));
        transform.parent.position = Camera.main.ViewportToWorldPoint(pos);
    }
}
