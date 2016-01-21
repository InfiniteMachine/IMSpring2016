using UnityEngine;
using System.Collections;

public class Character_Spawner : MonoBehaviour {

	// Look up arrays

	//these are the player prefabs that will be automatically plugged in for us.
	GameObject Tank01Prefab; 
	GameObject Tank02Prefab;
	GameObject Tank03Prefab;
	GameObject Tank04Prefab;

	//This is where the script placed in the level inputs in this number for the player who was selected and saved by playerPrefs
	int savedPlayer = 0;

	//this is called first before the Start function, so make sure it loads everything needed first.
	void Awake() 
	{

		savedPlayer = PlayerPrefs.GetInt("selectedPlayer");

		// This wouldnt work if you set up the scene with the GameObjects already inactive
		Tank01Prefab = GameObject.Find("Player1");
		Tank02Prefab = GameObject.Find("Player2");
		Tank03Prefab = GameObject.Find("Player3");
		Tank04Prefab = GameObject.Find("Player4");

		// Look up arrays 2.0
		if(savedPlayer == 0)
		{
			Tank01Prefab.SetActive(true);
			Tank02Prefab.SetActive(false);
			Tank03Prefab.SetActive(false);
			Tank04Prefab.SetActive(false);
		}

		else if(savedPlayer == 1)
		{
			Tank01Prefab.SetActive(true);
			Tank02Prefab.SetActive(false);
			Tank03Prefab.SetActive(false);
			Tank04Prefab.SetActive(false);
		}

		else if(savedPlayer == 2)
		{
			Tank01Prefab.SetActive(false);
			Tank02Prefab.SetActive(true);
			Tank03Prefab.SetActive(false);
			Tank04Prefab.SetActive(false);
		}

		else if(savedPlayer == 3)
		{
			Tank01Prefab.SetActive(false);
			Tank02Prefab.SetActive(false);
			Tank03Prefab.SetActive(true);
			Tank04Prefab.SetActive(false);
		}

		else if(savedPlayer == 4)
		{
			Tank01Prefab.SetActive(false);
			Tank02Prefab.SetActive(false);
			Tank03Prefab.SetActive(false);
			Tank04Prefab.SetActive(true);
		}
	}
}
