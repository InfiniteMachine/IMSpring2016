using UnityEngine;
using System.Collections;

public class explosion : MonoBehaviour
{
    public GameObject thing;

    // Use this for initialization
    void Awake()
    {

    }

    // Update is called once per frame
    void OnCollisionEnter(Collision other)
    {
        Instantiate(thing, other.transform.position, Quaternion.identity);
    }
}
