using UnityEngine;
using System.Collections.Generic;

public class Paralax : MonoBehaviour {
    private List<SpriteRenderer> srs;
    private float zero;
    private float width;

    private CamFollow cFollow;
    // Use this for initialization
	void Start () {
        srs = new List<SpriteRenderer>();
        foreach(Transform t in transform)
        {
            SpriteRenderer sr = t.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                srs.Add(sr);
            }
        }

        cFollow = Camera.main.GetComponent<CamFollow>();
        width = (cFollow.rightBounds - cFollow.leftBounds) / 2;
        zero = cFollow.leftBounds + width;

        Vector3 selfPosition = transform.position;
        selfPosition.y = (cFollow.upperBounds - cFollow.lowerBounds) / 2;
        selfPosition.x = zero;
        transform.position = selfPosition;

        for (int i = 0; i < srs.Count; i++)
        {
            Vector3 pos = srs[i].transform.position;
            pos.x = zero;
            srs[i].transform.position = pos;
        }


	}
	
	// Update is called once per frame
	void LateUpdate () {
        float offset = cFollow.transform.position.x - zero;
        for(int i = 0; i < srs.Count; i++)
        {
            float distance = srs[i].bounds.size.x - width;
            Vector3 pos = srs[i].transform.position;
            pos.x = distance * (offset / (2 * width)) + zero;
            srs[i].transform.position = pos;
        }
	}
}
