using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Pause : MonoBehaviour {
    public static Pause instance;
    private bool paused = false;

    private List<Text> buttons;
    private List<PauseScale> buttonAnimation;
    private Text playerDisplay;
    private int selectedButton = 0;
    public Color selected = Color.yellow;
    public Color normal = Color.white;
    private Controller controller;

    private float volume;
    void Awake()
    {
        instance = this;
        foreach (Transform t in transform)
            t.gameObject.SetActive(false);
        buttons = new List<Text>();
        buttonAnimation = new List<PauseScale>();
        buttons.Add(transform.FindChild("Resume").GetComponent<Text>());
        buttons.Add(transform.FindChild("MainMenu").GetComponent<Text>());
        buttons.Add(transform.FindChild("Quit").GetComponent<Text>());
        for (int i = 0; i < buttons.Count; i++) {
            buttonAnimation.Add(buttons[i].GetComponent<PauseScale>());
        }
        playerDisplay = transform.FindChild("PlayerDisplay").GetComponent<Text>();
    }

    void Update()
    {
        if (!paused)
            return;
        //Update
        if (controller.GetAxisAsButton(1, true) || controller.GetAxisAsButton(6, false))
        {
            //Down
            SoundManager.instance.PlayOneShot("Swap");
            buttonAnimation[selectedButton].SetState(false);
            selectedButton = (selectedButton + 1) % 3;
            buttonAnimation[selectedButton].SetState(true);
        }
        else if (controller.GetAxisAsButton(1, false) || controller.GetAxisAsButton(6, true))
        {
            //Up
            SoundManager.instance.PlayOneShot("Swap");
            buttonAnimation[selectedButton].SetState(false);
            selectedButton = (selectedButton - 1);
            if (selectedButton < 0)
                selectedButton = 2;
            buttonAnimation[selectedButton].SetState(true);
        }
        else if (controller.GetButtonDown(0))
        {
            //Submit
            SoundManager.instance.PlayOneShot("Select");
            if (selectedButton == 0)
                UnPause();
            else if (selectedButton == 1)
            {
                UnPause();
                Manager.instance.EndMatch(false);
            }
            else {
                Application.Quit();
                Debug.Log("Would quit if not in editor");
            }
        }
        else if (controller.GetButtonDown(1))
        {
            //Cancel
            UnPause();
        }
        //Update Colors
        buttons[0].text = selectedButton == 0 ? "[ Resume ]" : "Resume";
        buttons[0].color = selectedButton == 0 ? selected : normal;
        buttons[1].text = selectedButton == 1 ? "[ End Game ]" : "End Game";
        buttons[1].color = selectedButton == 1 ? selected : normal;
        buttons[2].text = selectedButton == 2 ? "[ Quit ]" : "Quit";
        buttons[2].color = selectedButton == 2 ? selected : normal;
    }

    public void PauseGame(int playerID, int controllerNumber)
    {
        Time.timeScale = 0;
        foreach (Transform t in transform)
            t.gameObject.SetActive(true);
        paused = true;
        selectedButton = 0;
        buttonAnimation[selectedButton].SetState(true);
        controller = ControllerPool.GetInstance().GetController(controllerNumber);
        playerDisplay.text = "Player " + (playerID + 1);
        SoundManager.instance.SetBackgroundVolume("backgroundMusic", 0.5f);
        SoundManager.instance.SetBackgroundVolume("TankMovement", 0f);
        Update();
    }

    private void UnPause()
    {
        Time.timeScale = 1;
        foreach (Transform t in transform)
            t.gameObject.SetActive(false);
        for (int i = 0; i < buttonAnimation.Count; i++)
            buttonAnimation[i].SetState(false);
        SoundManager.instance.SetBackgroundVolume("backgroundMusic", 1f);
        SoundManager.instance.SetBackgroundVolume("TankMovement", SoundManager.instance.backgroundClips[1].defautVolume);
        paused = false;
    }
}
