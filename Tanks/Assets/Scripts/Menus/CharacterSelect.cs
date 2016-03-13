using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class CharacterSelect : MonoBehaviour {
    private enum MenuState { CHARACTER, SCENE, MATCH_OPTIONS}
    private MenuState curMenu = MenuState.CHARACTER;
    private CanvasGroup playerSelect;
    public Sprite[] characterArt;
    private enum PStates { DISABLED, CHOOSING, LOCKED };
    private class Podium
    {
        public PStates state;
        public Image display;
        public Text name;
        public Image disabled;
    }
    private Podium[] podiums;
    private CanvasGroup sceneSelect;
    public string[] areaNames;
    public Sprite[] scenes;
    private int sceneChooser = -1;
    private int arena = 0;
    private Image arenaDisplay;
    private Text playerDisplay;
    private Text arenaNameDisplay;
    private CanvasGroup matchOptions;
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
        Manager.instance.numPlayers = 0;
        for(int i = 0; i < 4; i++)
            Manager.instance.playerTanks[i] = 0;
        playerSelect = transform.FindChild("PlayerSelect").GetComponent<CanvasGroup>();
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
        }
        if(scenes.Length != areaNames.Length)
            Debug.Log("There is a mismatch of artwork and arena names");
        playerDisplay = sceneSelect.transform.FindChild("SelectPlayer").GetComponent<Text>();
        arenaDisplay = sceneSelect.transform.FindChild("LevelArt").GetComponent<Image>();
        arenaNameDisplay = sceneSelect.transform.FindChild("LevelName").GetComponent<Text>();
        matchOptions = transform.FindChild("MatchOptions").GetComponent<CanvasGroup>();
        matchOptions.alpha = 0;
        matchOptions.interactable = false;
        matchOptions.blocksRaycasts = false;
        UpdatePodiums();
        UpdateSceneSelectVisual();
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
                        if (controllers[i].GetAxisAsButton(0, true) || controllers[i].GetAxisAsButton(5, false))
                        {
                            //Right
                            Manager.instance.playerTanks[p] += 1;
                            if (Manager.instance.playerTanks[p] >= characterArt.Length)
                                Manager.instance.playerTanks[p] = 0;
                            SoundManager.instance.PlayOneShot("Swap");
                        }
                        else if (controllers[i].GetAxisAsButton(0, false) || controllers[i].GetAxisAsButton(5, true))
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
                    break;
                case PStates.CHOOSING:
                    podiums[i].display.enabled = true;
                    podiums[i].display.sprite = characterArt[Manager.instance.playerTanks[i]];
                    podiums[i].display.color = Color.white;
                    podiums[i].disabled.enabled = false;
                    podiums[i].name.text = Manager.instance.tanks[Manager.instance.playerTanks[i]].GetComponent<PlayerController>().characterName;
                    break;
                case PStates.LOCKED:
                    podiums[i].display.color = Color.gray;
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
            }else if (Input.GetKeyDown(KeyCode.X))
            {
                StartCoroutine(SwapMenu(MenuState.MATCH_OPTIONS));
                SoundManager.instance.PlayOneShot("Select");
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
                SoundManager.instance.PlayOneShot("Swap");
            }
            else if (c.GetAxisAsButton(0, false) || c.GetAxisAsButton(5, true))
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

    private IEnumerator SwapMenu(MenuState newMenu)
    {
        animating = true;
        float counter = 0;
#if UNITY_EDITOR
        Debug.Log(curMenu.ToString() + " to " + newMenu.ToString());
#endif
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