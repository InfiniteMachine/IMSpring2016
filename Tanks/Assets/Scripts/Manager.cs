using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class Manager : MonoBehaviour {
	// I could load these in dynamically from Resources but lets not unless we add a bunch more tanks
	[Tooltip("Should contain the prefabs of all the tanks.")]
	public GameObject[] tanks;
	[HideInInspector]
    public int[] playerTanks;
    public Sprite[] playerPortraits;
    [HideInInspector]
    public int[] playerControllers;
    [HideInInspector]
    public int numPlayers = 0;
    private Text[] scoreDisplays;
    private Text timer;
    private GameObject scorePanel;
    [Header("Setup Match Info:")] // Match info for setup, not used in active game
    public int maxPlayers = 4;
    public float endTime = 300f;
    public float targetScore = 150f;
    public Color[] playerColors = new Color[] { Color.green, Color.red, Color.blue, Color.yellow };
    public GameObject[] trailParticles;
    public Sprite[] playerDisplayTags;
    public Sprite[] playerTags;
    [Header("Active Match Info:")]
	// Variables for an active game
	public bool activeMatch = false;
	public float time = 0f;
	public float[] indScore;
    public int[] kills;
    public int[] deaths;
    private Spawn[] spawns;
    private GameObject[] batonLocations;
    private int playerWithBaton = -1;
    private GameObject baton;
    [HideInInspector]
	static public Manager instance;
    private bool initOnLoad = false;
    public float scoreScreenDuration = 10f;

    private int lastSecond = -1;

    public enum GameModes { KINGME= 0, BLITZKRIEG, ANARCHY };
    [HideInInspector]
    public GameModes gameMode = GameModes.KINGME;
    [HideInInspector]
    public float moveMultiplier = 1;
    [HideInInspector]
    public float gravityScale = 1;
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
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
		if(activeMatch)
		{
			time += Time.deltaTime;
			if(time>endTime)
			{
                if(gameMode != GameModes.ANARCHY)
                    ApplyScore(endTime - (time - Time.deltaTime));
				EndMatch(true);
			}
			else
			{
                if (gameMode != GameModes.ANARCHY)
                    ApplyScore(Time.deltaTime);
                if (targetScore != 0)
                {
                    for (int i = 0; i < numPlayers; i++)
                    {
                        if (indScore[i] >= targetScore)
                            EndMatch(false);
                    }
                }
                if (endTime - time > 60)
                {
                    timer.text = string.Format("{0:0}:{1:00}", (int)(endTime - time) / 60, (int)(endTime - time) % 60);
                }
                else
                {
                    timer.text = (Mathf.RoundToInt((endTime - time) * 10) / 10f).ToString();
                    if (!timer.text.Contains("."))
                        timer.text += ".0";
                    if (endTime - time < 10)
                    {
                        timer.text = " " + timer.text;
                        if(endTime - time < 6)
                        {
                            int value = Mathf.RoundToInt(endTime - time);
                            if(value != lastSecond && value > 0)
                            {
                                lastSecond = value;
                                SoundManager.instance.PlayOneShot("timer_beep");
                            }
                        }
                    }
                }
            }
        }
	}

	public void StartMatch()
	{
		SpawnTanks();
		activeMatch = true;
		time = 0f;

        indScore = new float[numPlayers];
        kills = new int[numPlayers];
        deaths = new int[numPlayers];

        for (int x=0;x<indScore.Length;x++)
			indScore[x] = 0f;
        SoundManager.instance.PlayBackground("backgroundMusic");
	}

	public string GetScore(int ID)
	{
		float returnScore = indScore[ID];
		return (Mathf.Floor(returnScore*10) / 10).ToString();
	}

	public void EndMatch(bool timeout = false)
	{
		activeMatch = false;
        for (int i = 0; i < numPlayers; i++)
            scoreDisplays[i].transform.parent.gameObject.SetActive(false);
        for (int i = numPlayers + 1; i <= 4; i++)
        {
            scorePanel.transform.FindChild("" + (i)).gameObject.SetActive(false);
            scorePanel.transform.FindChild("Character" + (i)).gameObject.SetActive(false);
            scorePanel.transform.FindChild("Crown" + (i)).gameObject.SetActive(false);
        }
        timer.gameObject.SetActive(false);
        timer.transform.parent.FindChild("TimeBackground").gameObject.SetActive(false);
        timer.transform.parent.FindChild("TargetScore").gameObject.SetActive(false);
        scorePanel.SetActive(true);
        List<int> players = new List<int>();
        players.Add(0);
        for(int i = 1; i < numPlayers; i++)
        {
            bool insert = false;
            for(int j = 0; j < players.Count; j++)
            {
                if (indScore[i] > indScore[players[j]])
                {
                    players.Insert(j, i);
                    insert = true;
                    break;
                }
                else if (indScore[i] == indScore[players[j]])
                {
                    //Sort by kills
                    if(kills[i] > kills[players[j]])
                    {
                        players.Insert(j, i);
                        insert = true;
                        break;
                    }else if(kills[i] == kills[players[j]])
                    {
                        if(deaths[i] < deaths[players[j]])
                        {
                            players.Insert(j, i);
                            insert = true;
                            break;
                        }
                    }
                }
            }
            if (!insert)
                players.Add(i);
        }
        for (int i = 0; i < players.Count; i++)
        {
            scorePanel.transform.FindChild("Character" + (i + 1)).GetComponent<Image>().sprite = playerPortraits[playerTanks[players[i]]];
            Transform placeHolder = scorePanel.transform.FindChild("" + (i + 1));
            placeHolder.FindChild("PlayerTag").GetComponent<Image>().sprite = playerDisplayTags[players[i]];
            placeHolder.FindChild("Score").GetComponent<Text>().text = "" + Mathf.RoundToInt(indScore[players[i]]);
            placeHolder.FindChild("Kills").GetComponent<Text>().text = "" + kills[players[i]];
            placeHolder.FindChild("Deaths").GetComponent<Text>().text = "" + deaths[players[i]];
            if (deaths[players[i]] > 0)
            {
                placeHolder.FindChild("Ratio").GetComponent<Text>().text = "" + (Mathf.Round(((float)kills[players[i]] / (float)deaths[players[i]]) * 100f) / 100f);
            }
            else
                placeHolder.FindChild("Ratio").GetComponent<Text>().text = "" + kills[players[i]];
        }
        SoundManager.instance.StopAll();
        Invoke("GotoMenu", scoreScreenDuration);
	}

	public void ApplyScore(float time)
	{
        if (playerWithBaton >= 0 && playerWithBaton < numPlayers)
        {
            indScore[playerWithBaton] += time;
            scoreDisplays[playerWithBaton].text = Mathf.RoundToInt(indScore[playerWithBaton]).ToString();
        }
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
        baton.GetComponent<BatonController>().Activate();
	}

	public bool SpawnTanks()
	{
        Physics2D.gravity = Vector2.down * 9.81f * gravityScale;
        // Assumed that any spawn may accept any player regardless of number of players
        batonLocations = GameObject.FindGameObjectsWithTag("BatonLocation");
        baton = GameObject.FindGameObjectWithTag("Baton");
        if (gameMode == GameModes.ANARCHY)
            Destroy(baton);
        ResetBaton(true);
        GameObject[] newPlayerObjects = new GameObject[numPlayers];
		GameObject[] spawnLocations = GameObject.FindGameObjectsWithTag("Spawn");
        spawns = new Spawn[spawnLocations.Length];
        for(int i = 0; i < spawnLocations.Length; i++)
        {
            spawns[i] = spawnLocations[i].GetComponent<Spawn>();
        }
		if(spawns.Length< numPlayers)
		{
			Debug.LogError("Not enough spawns for players");
			return false;
		}

        GameObject screenCanvas = GameObject.FindGameObjectWithTag("UICanvas");
        Canvas screen = screenCanvas.GetComponent<Canvas>();
        screen.renderMode = RenderMode.ScreenSpaceCamera;
        screen.worldCamera = Camera.main;
        screen.planeDistance = 1f;
        scorePanel = screenCanvas.transform.FindChild("ScoresPanel").gameObject;
        scorePanel.SetActive(false);
        for (int i = numPlayers + 1; i <= 4; i++)
        {
            screenCanvas.transform.FindChild("Score" + i + "Panel").gameObject.SetActive(false);
        }
        scoreDisplays = new Text[numPlayers];
        for(int i = 0; i < numPlayers; i++)
        {
            scoreDisplays[i] = screenCanvas.transform.FindChild("Score" + (i + 1) + "Panel").FindChild("Text").GetComponent<Text>();
            scoreDisplays[i].text = 0 + "";
        }
        timer = screenCanvas.transform.FindChild("TimeBackground/Timer").GetComponent<Text>();
        if (targetScore == 0)
        {
            Destroy(screenCanvas.transform.FindChild("TimeBackground/TargetScore").gameObject);
        }
        else
            screenCanvas.transform.FindChild("TimeBackground/TargetScore").GetComponent<Text>().text = "" + targetScore;
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
            newPlayerObjects[x].GetComponent<PlayerController>().SetPlayerID(x);
            newPlayerObjects[x].GetComponent<PlayerController>().controllerNumber = playerControllers[x];

            newPlayerObjects[x].transform.FindChild("Tag").GetComponent<SpriteRenderer>().sprite = playerTags[x];
            GameObject go = Instantiate(trailParticles[x]);
            go.transform.SetParent(newPlayerObjects[x].transform);
            Vector3 pos = Vector3.zero;
            pos.z = 0.1f;
            go.transform.localPosition = pos;
            screenCanvas.transform.FindChild("Score" + (x + 1) + "Panel").FindChild("Image").GetComponent<Image>().sprite = playerPortraits[playerTanks[x]];//newPlayerObjects[x].GetComponent<SpriteRenderer>().sprite;
            newPlayerObjects[x].GetComponent<SpriteRenderer>().material.SetColor("_Color", playerColors[x]);
        }
		return true;
	}

    public Vector3 GetRandomSpawn()
    {
        int selected = Random.Range(0, spawns.Length);
        while (!spawns[selected].IsEmpty())
        {
            selected = Random.Range(0, spawns.Length);
        }
        return spawns[selected].transform.position;
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

    private void GotoMenu()
    {
        SoundManager.instance.StopBackground("backgroundMusic");
        SceneManager.LoadScene("Menu");
    }

    public void RecordDeath(int killer, int target)
    {
        if (target != killer)
            kills[killer]++;
        deaths[target]++;
        if (gameMode == GameModes.ANARCHY)
        {
            indScore[killer]++;
            scoreDisplays[killer].text = Mathf.RoundToInt(indScore[killer]).ToString();
        }
    }
}