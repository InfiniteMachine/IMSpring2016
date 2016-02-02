using UnityEngine;
using System.Collections;

public class EmptySceneHandler : MonoBehaviour {
    public GameObject tankPrefab;
	void Start () {
        if (Manager.instance == null)
        {
            GameObject go = (GameObject)Instantiate(tankPrefab, GameObject.FindGameObjectWithTag("Spawn").transform.position, Quaternion.identity);
            go.GetComponent<PlayerController>().controllerNumber = -1;
        }
        Destroy(this);
	}
}
