using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour {

	// I could load these in dynamically from Resources but lets not unless we add a bunch more tanks
	public GameObject[] tanks;

	[Header("Match Info Below")]
	public bool matchActive = false;
	public Vector3 middlePoint;
	public int[] playerTanks;
	public GameObject[] activeTanks;
	public bool[] faceRight;

	[HideInInspector]
	static public Manager instance;

	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad(gameObject);
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartMatch()
	{
		SpawnTanks();
		matchActive = true;
	}

	public void EndMatch()
	{
		matchActive = false;
	}

	public bool SpawnTanks()
	{
		// Assumed that spawns are somewhat symmetrical
		// Assumed that any spawn may accept any player regardless of number of players

		int players = playerTanks.Length;
		GameObject[] newPlayerObjects = new GameObject[players];
		GameObject[] spawns = GameObject.FindGameObjectsWithTag("Spawn");

		if(spawns.Length<players)
		{
			Debug.LogError("Not enough spawns for players");
			return false;
		}

		int l = spawns.Length;
		for(int x=0;x<spawns.Length;x++)
		{
			middlePoint += spawns[x].transform.position / (float) l;
		}

		bool[] spawnUsed = new bool[spawns.Length];
		for(int x=0;x<spawnUsed.Length;x++)
			spawnUsed[x] = false;

		Vector3 nextSpawn;
		int randInt;
		for(int x=0;x<players;x++)
		{
			// Find spawn point
			while(true)
			{
				randInt = Random.Range(0, l);
				if(!spawnUsed[randInt])
				{
					nextSpawn = spawns[randInt].transform.position;
					spawnUsed[randInt] = true;
					break;
				}
			}

			// Spawn new tank
			newPlayerObjects[x] = (GameObject) GameObject.Instantiate(tanks[playerTanks[x]], nextSpawn, Quaternion.identity);

			// Put something here that gives tank an ID, and whatever else is needed.
		}

		return true;
	}
}
