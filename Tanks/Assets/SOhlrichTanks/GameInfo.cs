using UnityEngine;
using System.Collections;

public class GameInfo : MonoBehaviour {

	// This script's variables were folded into Manager.cs

	public bool matchActive = false;
	public int[] playerTanks;
	public GameObject[] activeTanks;
	public bool[] faceRight;

	static public GameInfo instance;

	// Use this for initialization
	void Awake () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
