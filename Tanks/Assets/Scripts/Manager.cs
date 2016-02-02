using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour {
	// I could load these in dynamically from Resources but lets not unless we add a bunch more tanks
	[Tooltip("Should contain the prefabs of all the tanks.")]
	public GameObject[] tanks;

	[HideInInspector]
    public int[] playerTanks;
    [HideInInspector]
    public int[] playerControllers;
    [HideInInspector]
    public int numPlayers = 0;
    [Header("Setup Match Info:")] // Match info for setup, not used in active game
    public int maxPlayers = 4;
    public float endTime = 150f;
	public float endScore = 150f;

	[Header("Active Match Info:")]
	// Variables for an active game
	public bool activeMatch = false;
	public float time = 0f;
	public float[] indScore;
    private GameObject[] spawns;
    private GameObject[] batonLocations;
    private int playerWithBaton = -1;
    private GameObject baton;
    [HideInInspector]
	static public Manager instance;
    private bool initOnLoad = false;

    
	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad(gameObject);
		instance = this;
        playerTanks = new int[maxPlayers];
        playerControllers = new int[maxPlayers];
        for (int i = 0; i < maxPlayers; i++)
        {
            playerTanks[i] = -1;
            playerControllers[i] = -1;
        }
	}
	
	// Update is called once per frame
	void Update () {
		if(activeMatch)
		{
			time += Time.deltaTime;
			if(time>endTime)
			{
				ApplyScore(endTime - (time - Time.deltaTime));
				EndMatch(true);
			}
			else
			{
				ApplyScore(Time.deltaTime);
			}
		}
	}

	public void StartMatch()
	{
		SpawnTanks();
		activeMatch = true;
		time = 0f;

		indScore = new float[numPlayers];
		for(int x=0;x<indScore.Length;x++)
			indScore[x] = 0f;
	}

	public string GetScore(int ID)
	{
		float returnScore = indScore[ID];
		return (Mathf.Floor(returnScore*10) / 10).ToString();
	}

	public void EndMatch(bool timeout = false)
	{
		activeMatch = false;
	}

	public void ApplyScore(float time)
	{
        if(playerWithBaton >= 0 && playerWithBaton < numPlayers)
            indScore[playerWithBaton] += time;
	}

	public void GiveBaton(int playerID)
	{
        playerWithBaton = playerID;	
	}

	public bool HasBaton(int playerID)
	{
        return playerWithBaton == playerID;	
	}

	public void ResetBaton(bool resetBatonPosition)
	{
        if(resetBatonPosition)
            baton.transform.position = batonLocations[Random.Range(0, batonLocations.Length)].transform.position;
        playerWithBaton = -1;
        baton.GetComponent<Collider2D>().enabled = true;
	}

	public bool SpawnTanks()
	{
        // Assumed that spawns are somewhat symmetrical
        // Assumed that any spawn may accept any player regardless of number of players
        batonLocations = GameObject.FindGameObjectsWithTag("BatonLocation");
        baton = GameObject.FindGameObjectWithTag("Baton");
        ResetBaton(true);
        GameObject[] newPlayerObjects = new GameObject[numPlayers];
		spawns = GameObject.FindGameObjectsWithTag("Spawn");
		if(spawns.Length< numPlayers)
		{
			Debug.LogError("Not enough spawns for players");
			return false;
		}

		bool[] spawnUsed = new bool[spawns.Length];
		for(int x=0;x<spawnUsed.Length;x++)
			spawnUsed[x] = false;

		Vector3 nextSpawn;
		int randInt;
        for (int x = 0; x < numPlayers; x++)
        {
            // Find spawn point
            while (true)
            {
                randInt = Random.Range(0, spawns.Length);
                if (!spawnUsed[randInt])
                {
                    nextSpawn = spawns[randInt].transform.position;
                    spawnUsed[randInt] = true;
                    break;
                }
            }

            // Spawn new tank
            newPlayerObjects[x] = (GameObject)Instantiate(tanks[playerTanks[x]], nextSpawn, Quaternion.identity);
            newPlayerObjects[x].GetComponent<PlayerController>().playerID = x;
            newPlayerObjects[x].GetComponent<PlayerController>().controllerNumber = playerControllers[x];
        }
		return true;
	}

    public Vector3 GetSpawn(int spawn = -1)
    {
        if (spawn == -1)
            return spawns[Random.Range(0, spawns.Length)].transform.position;
        else
            return spawns[spawn].transform.position;
    }

    public void InitOnNextScene()
    {
        initOnLoad = true;
    }

    void OnLevelWasLoaded(int level)
    {
        if (initOnLoad)
        {
            StartMatch();
            initOnLoad = false;
        }
    }
}