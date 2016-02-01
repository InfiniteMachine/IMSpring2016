using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour {
    private enum MenuState { CHARACTER, SCENE}
    private MenuState curMenu = MenuState.CHARACTER;
    private GameObject playerSelect;
    public Sprite[] characterArt;
    public Sprite disabledArt;
    private enum PStates { DISABLED, CHOOSING, LOCKED };
    private PStates[] pStates;
    private Image[] podiums;
    
    private GameObject sceneSelect;
    public string[] areaNames;
    public Sprite[] scenes;
    private int sceneChooser = -1;
    private int arena = 0;
    private Image arenaDisplay;
    private Text playerDisplay;
    private Text arenaNameDisplay;
    // Update is called once per frame
    void Start()
    {
        playerSelect = transform.FindChild("PlayerSelect").gameObject;
        sceneSelect = transform.FindChild("SceneSelect").gameObject;
        sceneSelect.SetActive(false);
        pStates = new PStates[4];
        for (int i = 0; i < pStates.Length; i++)
            pStates[i] = PStates.DISABLED;
        podiums = new Image[4];
        for(int i = 0; i < podiums.Length; i++)
            podiums[i] = playerSelect.transform.FindChild("Tank" + (i + 1)).GetComponent<Image>();
        if(scenes.Length != areaNames.Length)
            Debug.Log("There is a mismatch of artwork and arena names");
        playerDisplay = sceneSelect.transform.FindChild("SelectPlayer").GetComponent<Text>();
        arenaDisplay = sceneSelect.transform.FindChild("LevelArt").GetComponent<Image>();
        arenaNameDisplay = sceneSelect.transform.FindChild("LevelName").GetComponent<Text>();
        UpdatePodiums();
    }

    void Update()
    {
        switch (curMenu)
        {
            case MenuState.CHARACTER:
                CharacterSelectUpdate();
                break;
            case MenuState.SCENE:
                SceneSelectUpdate();
                break;
        }
    }

    public void CharacterSelectUpdate()
    {
        Controller[] controllers = ControllerPool.GetInstance().GetControllers();
        for (int i = 0; i < controllers.Length; i++)
        {
            bool usedController = false;
            for (int p = 0; p < Manager.instance.numPlayers; p++)
            {
                if (Manager.instance.playerControllers[p] == (i + 1))
                {
                    //Player specific update
                    usedController = true;
                    if (pStates[i] == PStates.CHOOSING)
                    {
                        if (controllers[i].GetAxisAsButton(0, true) || controllers[i].GetAxisAsButton(5, false))
                        {
                            //Right
                            Manager.instance.playerTanks[p] += 1;
                            if (Manager.instance.playerTanks[p] >= characterArt.Length)
                                Manager.instance.playerTanks[p] = 0;
                        }
                        else if (controllers[i].GetAxisAsButton(0, false) || controllers[i].GetAxisAsButton(5, true))
                        {
                            //Right's demon twin
                            Manager.instance.playerTanks[p] -= 1;
                            if (Manager.instance.playerTanks[p] < 0)
                                Manager.instance.playerTanks[p] = characterArt.Length - 1;
                        }
                        else if (controllers[i].GetButtonDown(0))
                        {
                            //Submit
                            pStates[p] = PStates.LOCKED;
                        }
                    }
                    else if (controllers[i].GetButtonDown(1))
                    {
                        //Cancel
                        pStates[p] = PStates.CHOOSING;
                    }
                }
            }
            if (!usedController)
            {
                if (controllers[i].GetButtonDown(0))
                {
                    //Submit
                    Manager.instance.playerControllers[Manager.instance.numPlayers] = i + 1;
                    Manager.instance.playerTanks[Manager.instance.numPlayers] = 0;
                    pStates[Manager.instance.numPlayers] = PStates.CHOOSING;
                    Manager.instance.numPlayers++;
                }
            }
        }
        bool usedKeyboard = false;
        for (int p = 0; p < Manager.instance.numPlayers; p++)
        {
            if (Manager.instance.playerControllers[p] == -1)
            {
                usedKeyboard = true;
                //Player Specific Update
                if (pStates[p] == PStates.CHOOSING)
                {
                    if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        //Right's demon twin
                        Manager.instance.playerTanks[p] -= 1;
                        if (Manager.instance.playerTanks[p] < 0)
                            Manager.instance.playerTanks[p] = characterArt.Length - 1;
                    }
                    else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        //Right
                        Manager.instance.playerTanks[p] += 1;
                        if (Manager.instance.playerTanks[p] >= characterArt.Length)
                            Manager.instance.playerTanks[p] = 0;
                    }
                    else if (Input.GetKeyDown(KeyCode.Return))
                    {
                        //Submit
                        pStates[p] = PStates.LOCKED;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Escape))
                {
                    //Cancel
                    pStates[p] = PStates.CHOOSING;
                }

            }
        }
        if (!usedKeyboard)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                //Submit
                Manager.instance.playerControllers[Manager.instance.numPlayers] = -1;
                Manager.instance.playerTanks[Manager.instance.numPlayers] = 0;
                pStates[Manager.instance.numPlayers] = PStates.CHOOSING;
                Manager.instance.numPlayers++;
            }
        }

        if (Manager.instance.numPlayers > 1)
        {
            bool go = true;
            for (int p = 0; p < Manager.instance.numPlayers; p++)
            {
                go &= (pStates[p] == PStates.LOCKED);
            }
            if (go)
            {
                //Setup arena select screen
                curMenu = MenuState.SCENE;
                sceneChooser = Random.Range(0, Manager.instance.numPlayers);
                playerSelect.SetActive(false);
                sceneSelect.SetActive(true);
                UpdateSceneSelectVisual();
            }
        }
        UpdatePodiums();
    }

    public void UpdatePodiums()
    {
        for(int i = 0; i < podiums.Length; i++)
        {
            switch (pStates[i])
            {
                case PStates.DISABLED:
                    podiums[i].sprite = disabledArt;
                    podiums[i].color = Color.white;
                    break;
                case PStates.CHOOSING:
                    podiums[i].sprite = characterArt[Manager.instance.playerTanks[i]];
                    podiums[i].color = Color.white;
                    break;
                case PStates.LOCKED:
                    podiums[i].color = Color.gray;
                    break;
            }
        }
    }

    public void SceneSelectUpdate()
    {
        if (Manager.instance.playerControllers[sceneChooser] == -1)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                //Right's demon twin
                arena--;
                if (arena < 0)
                    arena = areaNames.Length - 1;
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                //Right
                arena++;
                if (arena >= areaNames.Length)
                    arena = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                //Submit
                GotoSelectedArena();
            }
        }
        else
        {
            Controller c = ControllerPool.GetInstance().GetController(Manager.instance.playerControllers[sceneChooser]);
            if (c.GetAxisAsButton(0, true) || c.GetAxisAsButton(5, false))
            {
                //Right
                arena++;
                if (arena >= areaNames.Length)
                    arena = 0;
            }
            else if (c.GetAxisAsButton(0, false) || c.GetAxisAsButton(5, true))
            {
                //Right's demon twin
                arena--;
                if (arena < 0)
                    arena = areaNames.Length - 1;
            }
            else if (c.GetButtonDown(0))
            {
                //Submit
                GotoSelectedArena();
            }
        }
        UpdateSceneSelectVisual();
    }

    public void UpdateSceneSelectVisual()
    {
        playerDisplay.text = "Player " + (sceneChooser + 1) + ", Select Arena";
        arenaDisplay.sprite = scenes[arena];
        arenaNameDisplay.text = areaNames[arena];
    }

    public void GotoSelectedArena()
    {
        Manager.instance.InitOnNextScene();
        SceneManager.LoadScene(areaNames[arena]);
    }
}