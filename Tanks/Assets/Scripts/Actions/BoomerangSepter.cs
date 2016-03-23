using UnityEngine;
using System.Collections;

public class BoomerangSepter : MonoBehaviour, IPlayerID {
    public float speed = 5;
    public float rotationSpeed = 60f;
    public float duration = 5f;
    private float durationCounter = 0f;
    private float direction = 1;
    private Transform t;

    private bool back = false;

    private int playerID;
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (back)
        {
            transform.position = Vector3.MoveTowards(transform.position, t.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, t.position) < 0.05f)
                Destroy(gameObject);
        }
        else
        {
            durationCounter += Time.deltaTime;
            if(durationCounter >= duration)
                back = true;
            else
                transform.position += (Vector3.right * speed * Time.deltaTime * direction);
        }
        transform.Rotate(0, 0, -direction * rotationSpeed * Time.deltaTime);
	}

    public void Setup(Transform player, int shootDirection)
    {
        t = player;
        direction = shootDirection;
    }

    public int GetPlayerID()
    {
        return playerID;
    }

    public void SetPlayerID(int id)
    {
        playerID = id;
    }
}
