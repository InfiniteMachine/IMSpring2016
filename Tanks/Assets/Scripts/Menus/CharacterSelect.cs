using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CharacterSelect : MonoBehaviour {
    private enum MenuState { CHARACTER, SCENE, MATCH_OPTIONS, LOADING}
    private MenuState curMenu = MenuState.CHARACTER;
    [Header("Player Chooser")]
    public Sprite[] characterArt;
    private CanvasGroup playerSelect;
    private enum PStates { DISABLED, CHOOSING, LOCKED };
    private class Podium
    {
        public PStates state;
        public Image display;
        public Image leftArrow, rightArrow;
        public Text name;
        public Image disabled;
    }
    private Podium[] podiums;
    private CanvasGroup sceneSelect;
    [Header("Scene Chooser")]
    public string[] areaNames;
    public Sprite[] scenes;
    private int sceneChooser = -1;
    private int arena = 0;
    private Dictionary<string, Image> images;
    private Dictionary<string, Text> texts;
    private CanvasGroup matchOptions;
    [Header("Match Options")]
    public Vector2 timeRange = new Vector2(300, 900);
    public float timeStep = 30f;
    public Vector2 moveRange = new Vector2(0.5f, 3f);
    public float moveStep = 0.5f;
    public Vector2 gravRange = new Vector2(0.5f, 3f);
    public float gravStep = 0.5f;
    public string[] gameModeNames = new string[] { "King Me", "Blitzkrieg", "Anarchy" };
    public string[] gameModeDesc = new string[] { "Maintain control of the crown for as long as possible.",
        "What makes war faster than being able to use attacks faster?",
        "There is no king. Be the best conqourer out there." };
    public Color activeColor = Color.yellow;
    public Color disabledColor = Color.gray;
    private int button = 0;
    private GameObject loading;
    private bool debug = false;
#if UNITY_EDITOR
    private string buttonMap = "2323";
    private int currentButton = 0;
#endif

    private bool animating = false;
    public float animationDuration = 0.5f;
    // Update is called once per frame
    void Start()
    {
        texts = new Dictionary<string, Text>();
        images = new Dictionary<string, Image>();
        Manager.instance.numPlayers = 0;
        for (int i = 0; i < 4; i++)
            Manager.instance.playerTanks[i] = 0;
        playerSelect = transform.FindChild("PlayerSelect").GetComponent<CanvasGroup>();
        playerSelect.alpha = 1;
        playerSelect.interactable = true;
        playerSelect.blocksRaycasts = true;
        loading = transform.FindChild("Loading").gameObject;
        loading.SetActive(false);
        sceneSelect = transform.FindChild("SceneSelect").GetComponent<CanvasGroup>();
        sceneSelect.alpha = 0;
        sceneSelect.interactable = false;
        sceneSelect.blocksRaycasts = false;
        Transform displays = playerSelect.transform.FindChild("CharacterDisplays");
        podiums = new Podium[4];
        for (int i = 0; i < podiums.Length; i++)
        {
            Transform tank = displays.FindChild("Tank" + (i + 1));
            podiums[i] = new Podium();
            podiums[i].state = PStates.DISABLED;
            podiums[i].display = tank.FindChild("Image").GetComponent<Image>();
            podiums[i].disabled = tank.FindChild("Disabled").GetComponent<Image>();
            podiums[i].name = tank.FindChild("Name").GetComponent<Text>();
            podiums[i].leftArrow = tank.FindChild("LeftArrow").GetComponent<Image>();
            podiums[i].rightArrow = tank.FindChild("RightArrow").GetComponent<Image>();
        }
        if(scenes.Length != areaNames.Length)
            Debug.Log("There is a mismatch of artwork and arena names");
        texts.Add("PlayerDisplay", sceneSelect.transform.FindChild("SelectPlayer").GetComponent<Text>());
        images.Add("ArenaDisplay", sceneSelect.transform.FindChild("LevelArt").GetComponent<Image>());
        texts.Add("ArenaNameDisplay", sceneSelect.transform.FindChild("LevelName").GetComponent<Text>());
        matchOptions = transform.FindChild("MatchOptions").GetComponent<CanvasGroup>();
        matchOptions.alpha = 0;
        matchOptions.interactable = false;
        matchOptions.blocksRaycasts = false;

        texts.Add("GameModeText", matchOptions.transform.FindChild("GameModeText").GetComponent<Text>());
        texts.Add("MatchDurationText", matchOptions.transform.FindChild("MatchDurationText").GetComponent<Text>());
        texts.Add("GameModeDesc", matchOptions.transform.FindChild("GameModeDesc").GetComponent<Text>());
        texts.Add("MatchDuration", matchOptions.transform.FindChild("MatchDuration/Text").GetComponent<Text>());
        texts.Add("GameMode", matchOptions.transform.FindChild("GameMode/Text").GetComponent<Text>());
        texts.Add("MovementMultiplier", matchOptions.transform.FindChild("MovementMultiplier/Text").GetComponent<Text>());
        texts.Add("MovementMultiplierText", matchOptions.transform.FindChild("MovementMultiplierText").GetComponent<Text>());
        texts.Add("GravityScale", matchOptions.transform.FindChild("GravityScale/Text").GetComponent<Text>());
        texts.Add("GravityScaleText", matchOptions.transform.FindChild("GravityScaleText").GetComponent<Text>());

        UpdatePodiums();
        UpdateSceneSelectVisual();
        UpdateMatchOptionsVisual();
    }

    void Update()
    {
        if (animating)
            return;
        switch (curMenu)
        {
            case MenuState.CHARACTER:
                CharacterSelectUpdate();
                break;
            case MenuState.SCENE:
                SceneSelectUpdate();
                break;
            case MenuState.MATCH_OPTIONS:
                UpdateMatchOptions();
                break;
        }
    }

    public void CharacterSelectUpdate()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            debug = !debug;
            Debug.Log("Debug Value Changed to: " + debug);
        }
        if (ControllerPool.GetInstance().GetButtonDown(buttonMap[currentButton] - '0'))
            currentButton++;
        if (currentButton >= buttonMap.Length)
        {
            debug = !debug;
            Debug.Log("Debug Value Changed to: " + debug);
            currentButton = 0;
        }
#endif
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
                    if (podiums[p].state == PStates.CHOOSING)
                    {
                        if (controllers[i].GetAxisAsButton(0, true) || controllers[i].GetAxisAsButton(5, true))
                        {
                            //Right
                            Manager.instance.playerTanks[p] += 1;
                            if (Manager.instance.playerTanks[p] >= characterArt.Length)
                                Manager.instance.playerTanks[p] = 0;
                            SoundManager.instance.PlayOneShot("Swap");
                        }
                        else if (controllers[i].GetAxisAsButton(0, false) || controllers[i].GetAxisAsButton(5, false))
                        {
                            //Right's demon twin
                            Manager.instance.playerTanks[p] -= 1;
                            if (Manager.instance.playerTanks[p] < 0)
                                Manager.instance.playerTanks[p] = characterArt.Length - 1;
                            SoundManager.instance.PlayOneShot("Swap");
                        }
                        else if (controllers[i].GetButtonDown(0))
                        {
                            //Submit
                            podiums[p].state = PStates.LOCKED;
                            SoundManager.instance.PlayOneShot("Select");
                        }
                    }
                    else if (controllers[i].GetButtonDown(1))
                    {
                        //Cancel
                        podiums[p].state = PStates.CHOOSING;
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
                    podiums[Manager.instance.numPlayers].state = PStates.CHOOSING;
                    Manager.instance.numPlayers++;
                    SoundManager.instance.PlayOneShot("Select");
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
                if (podiums[p].state == PStates.CHOOSING)
                {
                    if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        //Right's demon twin
                        Manager.instance.playerTanks[p] -= 1;
                        if (Manager.instance.playerTanks[p] < 0)
                            Manager.instance.playerTanks[p] = characterArt.Length - 1;
                        SoundManager.instance.PlayOneShot("Swap");
                    }
                    else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        //Right
                        Manager.instance.playerTanks[p] += 1;
                        if (Manager.instance.playerTanks[p] >= characterArt.Length)
                            Manager.instance.playerTanks[p] = 0;
                        SoundManager.instance.PlayOneShot("Swap");
                    }
                    else if (Input.GetKeyDown(KeyCode.Return))
                    {
                        //Submit
                        podiums[p].state = PStates.LOCKED;
                        SoundManager.instance.PlayOneShot("Select");
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
                {
                    //Cancel
                    podiums[p].state = PStates.CHOOSING;
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
                podiums[Manager.instance.numPlayers].state = PStates.CHOOSING;
                Manager.instance.numPlayers++;
                SoundManager.instance.PlayOneShot("Select");
            }
        }

        if (Manager.instance.numPlayers > 1 || (debug && Manager.instance.numPlayers > 0))
        {
            bool go = true;
            for (int p = 0; p < Manager.instance.numPlayers; p++)
            {
                go &= (podiums[p].state == PStates.LOCKED);
            }
            if (go)
            {
                //Setup arena select screen
                sceneChooser = Random.Range(0, Manager.instance.numPlayers);
                StartCoroutine(SwapMenu(MenuState.SCENE));
            }
        }
        UpdatePodiums();
    }

    public void UpdatePodiums()
    {
        for(int i = 0; i < podiums.Length; i++)
        {
            switch (podiums[i].state)
            {
                case PStates.DISABLED:
                    podiums[i].display.enabled = false;
                    podiums[i].display.color = Color.white;
                    podiums[i].disabled.enabled = true;
                    podiums[i].name.text = "";
                    podiums[i].leftArrow.enabled = false;
                    podiums[i].rightArrow.enabled = false;
                    break;
                case PStates.CHOOSING:
                    podiums[i].display.enabled = true;
                    podiums[i].display.sprite = characterArt[Manager.instance.playerTanks[i]];
                    podiums[i].display.color = Color.white;
                    podiums[i].disabled.enabled = false;
                    podiums[i].name.text = Manager.instance.tanks[Manager.instance.playerTanks[i]].GetComponent<PlayerController>().characterName;
                    podiums[i].leftArrow.enabled = true;
                    podiums[i].rightArrow.enabled = true;
                    break;
                case PStates.LOCKED:
                    podiums[i].display.color = Color.gray;
                    podiums[i].leftArrow.enabled = false;
                    podiums[i].rightArrow.enabled = false;
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
                SoundManager.instance.PlayOneShot("Swap");
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                //Right
                arena++;
                if (arena >= areaNames.Length)
                    arena = 0;
                SoundManager.instance.PlayOneShot("Swap");
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                //Submit
                GotoSelectedArena();
                SoundManager.instance.PlayOneShot("Select");
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                StartCoroutine(SwapMenu(MenuState.MATCH_OPTIONS));
                SoundManager.instance.PlayOneShot("Select");
            }
        }
        else
        {
            Controller c = ControllerPool.GetInstance().GetController(Manager.instance.playerControllers[sceneChooser]);
            if (c.GetAxisAsButton(0, true) || c.GetAxisAsButton(5, true))
            {
                //Right
                arena++;
                if (arena >= areaNames.Length)
                    arena = 0;
                SoundManager.instance.PlayOneShot("Swap");
            }
            else if (c.GetAxisAsButton(0, false) || c.GetAxisAsButton(5, false))
            {
                //Right's demon twin
                arena--;
                if (arena < 0)
                    arena = areaNames.Length - 1;
                SoundManager.instance.PlayOneShot("Swap");
            }
            else if (c.GetButtonDown(0))
            {
                //Submit
                GotoSelectedArena();
                SoundManager.instance.PlayOneShot("Select");
            }
            else if (c.GetButtonDown(2))
            {
                StartCoroutine(SwapMenu(MenuState.MATCH_OPTIONS));
                SoundManager.instance.PlayOneShot("Select");
            }
            else if (c.GetButtonDown(1))
            {
                for (int i = 0; i < podiums.Length; i++)
                {
                    if (podiums[i].state == PStates.LOCKED)
                        podiums[i].state = PStates.CHOOSING;
                }
                StartCoroutine(SwapMenu(MenuState.CHARACTER));
            }
        }
        UpdateSceneSelectVisual();
    }

    public void UpdateSceneSelectVisual()
    {
        texts["PlayerDisplay"].text = "Player " + (sceneChooser + 1) + ", Select Arena";
        images["ArenaDisplay"].sprite = scenes[arena];
        texts["ArenaNameDisplay"].text = areaNames[arena];
    }

    public void UpdateMatchOptions()
    {
        int leftRight = 0;
        int upDown = 0;
        bool cancel = false;
        Controller[] controllers = ControllerPool.GetInstance().GetControllers();
        for (int i = 0; i < controllers.Length; i++)
        {

            if (controllers[i].GetAxisAsButton(0, true) || controllers[i].GetAxisAsButton(5, true))
            {
                //Right
                SoundManager.instance.PlayOneShot("Swap");
                leftRight = 1;
            }
            else if (controllers[i].GetAxisAsButton(0, false) || controllers[i].GetAxisAsButton(5, false))
            {
                //Right's demon twin
                SoundManager.instance.PlayOneShot("Swap");
                leftRight = -1;
            }
            else if (controllers[i].GetAxisAsButton(1, true) || controllers[i].GetAxisAsButton(6, true))
            {
                //Down?
                SoundManager.instance.PlayOneShot("Swap");
                upDown = 1;
            }
            else if (controllers[i].GetAxisAsButton(1, false) || controllers[i].GetAxisAsButton(6, false))
            {
                //Up
                SoundManager.instance.PlayOneShot("Swap");
                upDown = -1;
            }
            else if (controllers[i].GetButtonDown(0))
            {
                //Submit
                SoundManager.instance.PlayOneShot("Swap");
                leftRight = 1;
            }
            else if (controllers[i].GetButtonDown(1))
            {
                //Cancel
                cancel = true;
            }
        }
        for(int i = 0; i < Manager.instance.numPlayers; i++)
        {
            if(Manager.instance.playerControllers[i] == -1)
            {
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    //Right
                    SoundManager.instance.PlayOneShot("Swap");
                    leftRight = -1;
                }
                else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    //Right's demon twin
                    SoundManager.instance.PlayOneShot("Swap");
                    leftRight = 1;
                }
                else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    //Down?
                    SoundManager.instance.PlayOneShot("Swap");
                    upDown = 1;
                }
                else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    //Up
                    SoundManager.instance.PlayOneShot("Swap");
                    upDown = -1;
                }
                else if (Input.GetKeyDown(KeyCode.Return))
                {
                    //Submit
                    SoundManager.instance.PlayOneShot("Swap");
                    leftRight = 1;
                }
                else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
                {
                    //Cancel
                    cancel = true;
                }
            }
        }
        if (cancel)
        {
            StartCoroutine(SwapMenu(MenuState.SCENE));
        }
        else if(leftRight != 0)
        {
            switch (button)
            {
                case 0:
                    if (leftRight < 0)
                    {
                        //Left
                        if (Manager.instance.gameMode == Manager.GameModes.KINGME)
                            Manager.instance.gameMode = Manager.GameModes.ANARCHY;
                        else
                            Manager.instance.gameMode -= 1;
                    }
                    else
                    {
                        //Right
                        if (Manager.instance.gameMode == Manager.GameModes.ANARCHY)
                            Manager.instance.gameMode = Manager.GameModes.KINGME;
                        else
                            Manager.instance.gameMode += 1;
                    }
                    break;
                case 1:
                    if(leftRight < 0)
                    {
                        //Left
                        if (Manager.instance.endTime <= timeRange.x)
                            Manager.instance.endTime = timeRange.y;
                        else
                            Manager.instance.endTime -= timeStep;
                    }
                    else
                    {
                        //Right
                        if (Manager.instance.endTime >= timeRange.y)
                            Manager.instance.endTime = timeRange.x;
                        else
                            Manager.instance.endTime += timeStep;
                    }
                    break;
                case 2:
                    if (leftRight < 0)
                    {
                        //Left
                        if (Manager.instance.moveMultiplier <= moveRange.x)
                            Manager.instance.moveMultiplier = moveRange.y;
                        else
                            Manager.instance.moveMultiplier -= moveStep;
                    }
                    else
                    {
                        //Right
                        if (Manager.instance.moveMultiplier >= moveRange.y)
                            Manager.instance.moveMultiplier = moveRange.x;
                        else
                            Manager.instance.moveMultiplier += moveStep;
                    }
                    break;
                case 3:
                    if (leftRight < 0)
                    {
                        //Left
                        if (Manager.instance.gravityScale <= gravRange.x)
                            Manager.instance.gravityScale = gravRange.y;
                        else
                            Manager.instance.gravityScale -= gravStep;
                    }
                    else
                    {
                        //Right
                        if (Manager.instance.gravityScale >= gravRange.y)
                            Manager.instance.gravityScale = gravRange.x;
                        else
                            Manager.instance.gravityScale += gravStep;
                    }
                    break;
            }
        }
        else if(upDown != 0)
        {
            button += upDown;
            if (button < 0)
                button = 3;
            else if (button > 3)
                button = 0;
        }
        UpdateMatchOptionsVisual();
    }

    public void UpdateMatchOptionsVisual()
    {
        texts["GameModeText"].color = (button == 0 ? activeColor : disabledColor);
        texts["GameMode"].color = (button == 0 ? activeColor : disabledColor);
        texts["GameMode"].text = gameModeNames[(int)Manager.instance.gameMode];

        texts["GameModeDesc"].text = gameModeDesc[(int)Manager.instance.gameMode];

        texts["MatchDurationText"].color = (button == 1 ? activeColor : disabledColor);
        texts["MatchDuration"].color = (button == 1 ? activeColor : disabledColor);
        texts["MatchDuration"].text = string.Format("{0:0}:{1:00}", (int)(Manager.instance.endTime / 60), (int)(Manager.instance.endTime % 60));

        texts["MovementMultiplierText"].color = (button == 2 ? activeColor : disabledColor);
        texts["MovementMultiplier"].color = (button == 2 ? activeColor : disabledColor);
        texts["MovementMultiplier"].text = string.Format("{0:0.0}x", Manager.instance.moveMultiplier);

        texts["GravityScaleText"].color = (button == 3 ? activeColor : disabledColor);
        texts["GravityScale"].color = (button == 3 ? activeColor : disabledColor);
        texts["GravityScale"].text = string.Format("{0:0.0}x", Manager.instance.gravityScale);
    }

    public void GotoSelectedArena()
    {
        sceneSelect.alpha = 0;
        sceneSelect.blocksRaycasts = false;
        sceneSelect.interactable = false;
        loading.SetActive(true);
        Manager.instance.InitOnNextScene();
        //SceneManager.LoadScene(areaNames[arena]);
        SceneManager.LoadSceneAsync(areaNames[arena]);
        curMenu = MenuState.LOADING;
        animating = true;
    }

    private IEnumerator SwapMenu(MenuState newMenu)
    {
        animating = true;
        float counter = 0;
        switch (curMenu)
        {
            case MenuState.CHARACTER:
                playerSelect.interactable = false;
                playerSelect.blocksRaycasts = false;
                break;
            case MenuState.SCENE:
                sceneSelect.interactable = false;
                sceneSelect.blocksRaycasts = false;
                break;
            case MenuState.MATCH_OPTIONS:
                matchOptions.interactable = false;
                matchOptions.blocksRaycasts = false;
                break;
        }
        while (counter < animationDuration / 2)
        {
            counter += Time.deltaTime;
            switch (curMenu)
            {
                case MenuState.CHARACTER:
                    playerSelect.alpha = (animationDuration - (2 * counter)) / animationDuration;
                    break;
                case MenuState.SCENE:
                    sceneSelect.alpha = (animationDuration - (2 * counter)) / animationDuration;
                    break;
                case MenuState.MATCH_OPTIONS:
                    matchOptions.alpha = (animationDuration - (2 * counter)) / animationDuration;
                    break;
            }
            yield return null;
        }
        curMenu = newMenu;
        counter = 0;
        while (counter < animationDuration / 2)
        {
            counter += Time.deltaTime;
            switch (curMenu)
            {
                case MenuState.CHARACTER:
                    playerSelect.alpha = (2 * counter) / animationDuration;
                    break;
                case MenuState.SCENE:
                    UpdateSceneSelectVisual();
                    sceneSelect.alpha = (2 * counter) / animationDuration;
                    break;
                case MenuState.MATCH_OPTIONS:
                    matchOptions.alpha = (2 * counter) / animationDuration;
                    break;
            }
            yield return null;
        }
        switch (curMenu)
        {
            case MenuState.CHARACTER:
                playerSelect.interactable = true;
                playerSelect.blocksRaycasts = true;
                UpdatePodiums();
                break;
            case MenuState.SCENE:
                sceneSelect.interactable = true;
                sceneSelect.blocksRaycasts = true;
                UpdateSceneSelectVisual();
                break;
            case MenuState.MATCH_OPTIONS:
                matchOptions.interactable = true;
                matchOptions.blocksRaycasts = true;
                break;
        }
        animating = false;
    }
}