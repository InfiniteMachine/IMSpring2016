using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadScene : MonoBehaviour {

	public string sceneName = "Name";

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
		SceneManager.LoadScene(sceneName);
	}
}
