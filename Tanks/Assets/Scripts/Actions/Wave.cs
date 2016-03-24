using UnityEngine;
using System.Collections;

public class Wave : MonoBehaviour {
    public float max = 1;
    public float duration = 1f;
    [HideInInspector]
    public int direction = 1;
    private float counter = 0;
    public GameObject freezer;
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        counter += Time.deltaTime;
        transform.localScale = new Vector3(Mathf.Lerp(0.25f, direction * max, counter / duration), 1, 1);
        if (counter >= duration)
            Destroy(gameObject);
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            GameObject go = (GameObject)Instantiate(freezer, col.transform.position, Quaternion.identity);
            go.GetComponent<Freezer>().player = col.gameObject;
        }
    }
}
