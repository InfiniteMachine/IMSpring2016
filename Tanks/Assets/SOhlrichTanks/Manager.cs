using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour {

	public enum GameMode{koth, oddball};

	// I could load these in dynamically from Resources but lets not unless we add a bunch more tanks
	[Tooltip("Should contain the prefabs of all the tanks.")]
	public GameObject[] tanks;

	[Header("Setup Match Info:")] // Match info for setup, not used in active game
	public GameMode gameMode = GameMode.koth;
	public bool teamGame = false;
	public int[] playerTanks;
	public float endTime = 150f;
	public float endScore = 150f;

	[Header("Active Match Info:")]
	// Variables for an active game
	public bool activeMatch = false;
	public float time = 0f;
	public GameObject[] activeTanks;
	public int[] playerTeamID;
	public float[] indScore;
	public float[] teamScore;
	ScoreBaton scorePointer;
	int batons;

	[HideInInspector]
	public bool[] faceRight;

	[HideInInspector]
	public Vector3 middlePoint;

	[HideInInspector]
	static public Manager instance;

	public class ScoreBaton {

		public delegate void ScoreDelegate(float newTime);
		static public ScoreDelegate ScoreDelegateCopy;

		public ScoreDelegate ApplyScore;
		public ScoreBaton next;
		public int player;


		public ScoreBaton(int newPlayer)
		{
			player = newPlayer;
			ApplyScore = ScoreDelegateCopy;
		}

		public void addBaton(int newPlayer)
		{
			next = new ScoreBaton(newPlayer);
		}

		public void KothScore(float time)
		{
			Manager.instance.indScore[player] += time;
		}

		public void OddballScore(float time)
		{
			Manager.instance.indScore[player] += time;
		}
	}

	void SetScoreFunc()
	{
		if(gameMode==GameMode.koth)
		{ScoreBaton.ScoreDelegateCopy = new ScoreBaton(-1).KothScore;}
		else if(gameMode==GameMode.oddball)
		{ScoreBaton.ScoreDelegateCopy = new ScoreBaton(-1).OddballScore;}
	}

	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad(gameObject);
		instance = this;
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

		indScore = new float[activeTanks.Length];
		for(int x=0;x<indScore.Length;x++)
			indScore[x] = 0f;

		teamScore = new float[activeTanks.Length];
		for(int x=0;x<teamScore.Length;x++)
			teamScore[x] = -1f;

		SetScoreFunc();
		ResetBaton();
	}

	public string GetScore(int ID, bool getTeamScore = false)
	{
		float returnScore;
		if(getTeamScore)
			returnScore = teamScore[ID];
		else returnScore = indScore[ID];
		return (Mathf.Floor(returnScore*10) / 10).ToString();
	}

	public void EndMatch(bool timeout = false)
	{
		activeMatch = false;
	}

	public void ApplyScore(float time)
	{
		ScoreBaton a = scorePointer;
		while(a!=null)
		{
			a.ApplyScore(time);
		}
		if(teamGame)
		{
			// Go through each player, add team scores.
			for(int x=0;x<teamScore.Length;x++)
				teamScore = -1f;

			for(int x=0;x<indScore.Length;x++)
			{
				teamScore[playerTeamID[x]] += indScore[x];
			}
		}
	}

	public void GiveBaton(int playerID, bool maxOneBaton = false)
	{
		if(batons==0)
		{
			scorePointer = new ScoreBaton(playerID);
			batons++;
		}
		else if(maxOneBaton && batons>=1)
		{
			if(HasBaton(playerID))
			{
				ScoreBaton a = scorePointer;
				while(batons!=1)
				{
					if(a==null)
						a = scorePointer;
					if(a.player!=playerID)
					{
						a = a.next;
						TakeBaton(a.player);
					}
				}
			}
			else
			{
				ResetBaton();
				GiveBaton(playerID, true);
			}
		}
		else
		{
			if(!HasBaton(playerID))
			{
				scorePointer.addBaton(playerID);
				batons++;
			}
		}
	}

	public bool TakeBaton(int playerID)
	{
		if(HasBaton(playerID))
		{
			if(scorePointer.player==playerID)
				scorePointer = scorePointer.next;
			
			ScoreBaton a = scorePointer;
			while(a!=null)
			{
				if(a.next.player==playerID)
				{
					a.next = a.next.next;
					break;
				}
				a = a.next;
			}
			return true;
		}
		return false;
	}

	public bool HasBaton(int playerID)
	{
		if(batons==0)
			return false;
		
		ScoreBaton a = scorePointer;
		while(a!=null)
		{
			if(a.player==playerID)
				return true;
			a = a.next;
		}
		return false;
	}

	public void ResetBaton()
	{
		scorePointer = null;
		batons = 0;
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
